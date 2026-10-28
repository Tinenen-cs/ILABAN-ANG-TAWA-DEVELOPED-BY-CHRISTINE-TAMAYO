[CmdletBinding()]
param([switch]$SelfTest,[switch]$Once,[string]$CodexPath='')
$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$lockPath=Join-Path $PSScriptRoot 'autoresume.lock'
$logPath=Join-Path $PSScriptRoot 'autoresume.log'
$statePath=Join-Path $PSScriptRoot 'autoresume-state.json'
$complete=Join-Path $repo 'TASK_COMPLETE.flag'
if(-not $CodexPath -or -not (Test-Path -LiteralPath $CodexPath)){$CodexPath=(Get-Command codex.exe -ErrorAction Stop).Source}
$instruction='Open this project folder, read RESUME_STATE.txt, HANDOFF.md, PENDING_NETWORK.md and CHECKLIST.md, run git status, finish any half-done step, then continue the original plan from the resume command. Do not ask questions. Do not start the queued final exe/Drive task unless the pre-check passes.'
function Safe-Text([string]$text){$text -replace '(?i)Bearer\s+\S+','Bearer [REDACTED]' -replace '(sk-|ghp_|github_pat_)[A-Za-z0-9_-]{12,}','[REDACTED]' -replace '(?i)(password|access_token|refresh_token|api_key)\s*[:=]\s*\S+','$1=[REDACTED]'}
function Write-Log([string]$message){Add-Content -LiteralPath $logPath -Encoding UTF8 -Value ((Get-Date).ToUniversalTime().ToString('yyyy-MM-dd HH:mm:ssZ')+' '+(Safe-Text $message))}
function Reset-Delay([string]$message,[datetime]$now=(Get-Date)){
    $match=[regex]::Match($message,'try again at\s+(?<time>\d{1,2}:\d{2}\s*[AP]M)','IgnoreCase')
    if(-not $match.Success){return 900}
    try{
        $time=[datetime]::ParseExact($match.Groups['time'].Value.Trim(),[string[]]@('h:mm tt','hh:mm tt','h:mmtt','hh:mmtt'),[Globalization.CultureInfo]::InvariantCulture,[Globalization.DateTimeStyles]::None)
        $reset=$now.Date.Add($time.TimeOfDay);if($reset -le $now){$reset=$reset.AddDays(1)}
        return [int][Math]::Ceiling(($reset.AddMinutes(2)-$now).TotalSeconds)
    }catch{return 900}
}
function Invoke-Agent([string]$prompt,[string]$sandbox='workspace-write',[switch]$Ephemeral){
    $start=New-Object Diagnostics.ProcessStartInfo;$start.FileName=$CodexPath
    $extra=if($Ephemeral){' --ephemeral'}else{''}
    $start.Arguments='exec --sandbox '+$sandbox+' --color never'+$extra+' -C "'+$repo+'" "'+$prompt.Replace('"','\"')+'"'
    $start.WorkingDirectory=$repo;$start.UseShellExecute=$false;$start.CreateNoWindow=$true
    $start.RedirectStandardInput=$true;$start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true
    $process=New-Object Diagnostics.Process;$process.StartInfo=$start
    if(-not $process.Start()){throw 'Codex process failed to start'}
    $process.StandardInput.Close();$stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
    $process.WaitForExit();$output=$stdout.Result+"`n"+$stderr.Result
    $result=[pscustomobject]@{Exit=$process.ExitCode;Output=$output};$process.Dispose();return $result
}
function Wait-Checkpoint([int]$seconds){
    $until=(Get-Date).AddSeconds($seconds)
    while((Get-Date) -lt $until -and -not (Test-Path -LiteralPath $complete)){Start-Sleep -Seconds ([Math]::Min(60,[Math]::Max(1,[int]($until-(Get-Date)).TotalSeconds)))}
}
function Save-State($state){$state|ConvertTo-Json|Set-Content -LiteralPath $statePath -Encoding UTF8}

if($SelfTest){
    $testLock=Join-Path $PSScriptRoot 'autoresume-selftest.lock';$handle=$null;$second=$null;$exclusive=$false
    try{$handle=[IO.File]::Open($testLock,'OpenOrCreate','ReadWrite','None');try{$second=[IO.File]::Open($testLock,'OpenOrCreate','ReadWrite','None')}catch{$exclusive=$true}}finally{if($second){$second.Dispose()};if($handle){$handle.Dispose()};Remove-Item -LiteralPath $testLock -Force -ErrorAction SilentlyContinue}
    if(-not $exclusive){throw 'Exclusive lock test failed'}
    $parsed=Reset-Delay 'Usage limit reached; try again at 9:15 AM' ([datetime]'2026-10-28 08:00:00')
    $fallback=Reset-Delay 'Usage limit; reset time unavailable' ([datetime]'2026-10-28 08:00:00')
    if($parsed -ne 4620 -or $fallback -ne 900){throw "Reset wait tests failed: $parsed / $fallback"}
    Write-Log 'SELF TEST: exclusive lock and reset-plus-two-minutes / 15-minute fallback passed.'
    $smoke=Invoke-Agent 'Reply with exactly AUTORESUME_SMOKE_OK. Do not run tools or change any files.' 'read-only' -Ephemeral
    if($smoke.Exit -ne 0 -or $smoke.Output -notmatch 'AUTORESUME_SMOKE_OK'){Write-Log ('SELF TEST CLI FAILED: '+$smoke.Output);throw 'Harmless CLI smoke failed; inspect autoresume.log'}
    Write-Log 'SELF TEST: harmless read-only codex exec returned AUTORESUME_SMOKE_OK, exit 0; no repo-editing agent was started.'
    [pscustomobject]@{utc=(Get-Date).ToUniversalTime().ToString('o');lock_exclusive=$true;reset_wait_seconds=$parsed;fallback_seconds=$fallback;harmless_cli_exit=$smoke.Exit;harmless_cli_passed=$true;codex=$CodexPath}|ConvertTo-Json|Set-Content (Join-Path $repo 'Docs/QA_EVIDENCE/autoresume_tests.json') -Encoding UTF8
    Write-Output 'AUTORESUME SELF TEST PASSED';exit 0
}

$handle=$null
try{$handle=[IO.File]::Open($lockPath,'OpenOrCreate','ReadWrite','None')}catch{Write-Log 'Another watchdog owns the lock; this copy exits.';exit 0}
try{
    $bytes=[Text.Encoding]::UTF8.GetBytes("PID=$PID UTC=$((Get-Date).ToUniversalTime().ToString('o'))");$handle.SetLength(0);$handle.Write($bytes,0,$bytes.Length);$handle.Flush()
    $state=if(Test-Path -LiteralPath $statePath){Get-Content -LiteralPath $statePath -Raw|ConvertFrom-Json}else{[pscustomobject]@{launches=0;failures=0;halted=$false;nextAttemptUtc='';reason='';watchdogPid=$PID}}
    $state.watchdogPid=$PID;Save-State $state
    if($state.halted){Write-Log ('Persisted three-failure halt: '+$state.reason);exit 1}
    Write-Log "Watchdog started in $repo; relaunch count=$($state.launches)."
    $offlineAttempts=0;$backoff=@(5,15,30,60,120,300,600,600)
    while(-not (Test-Path -LiteralPath $complete)){
        # Fail closed: desktop/IDE servers do not expose their busy state to this script.
        # Any existing Codex process blocks launch, including an idle App/IDE host.
        $busy=@(Get-CimInstance Win32_Process -Filter "Name='codex.exe'" -ErrorAction Stop)
        if($busy.Count -gt 0){Write-Log ('Codex process present; conservative one-agent guard skips launch. PIDs='+($busy.ProcessId -join ','));if($Once){break};Wait-Checkpoint 60;continue}
        if($state.nextAttemptUtc -and (Get-Date).ToUniversalTime() -lt [datetime]::Parse($state.nextAttemptUtc).ToUniversalTime()){
            if($Once){Write-Log ('Waiting for stored reset time '+$state.nextAttemptUtc);break};Wait-Checkpoint 60;continue
        }
        $online=Test-NetConnection github.com -Port 443 -InformationLevel Quiet -WarningAction SilentlyContinue
        if(-not $online){$delay=$backoff[[Math]::Min($offlineAttempts,7)];$offlineAttempts++;Write-Log "Offline connectivity check failed; retry $offlineAttempts in $delay seconds; no launch.";if($Once){break};Wait-Checkpoint $delay;continue}
        $offlineAttempts=0;$state.launches++;$state.reason='Online; no existing Codex process';Save-State $state
        Write-Log "Launching checkpoint continuation number $($state.launches)."
        try{$run=Invoke-Agent $instruction}catch{$run=[pscustomobject]@{Exit=-1;Output=('ERROR: launcher failed: '+$_.Exception.Message)}}
        $diagnostics=([regex]::Matches($run.Output,'(?im)^.*(?:error:|exception|usage limit|rate limit|try again at|failed).*$')|ForEach-Object {$_.Value}) -join "`n"
        Safe-Text $diagnostics|Set-Content (Join-Path $PSScriptRoot 'autoresume-last-output.txt') -Encoding UTF8
        if($run.Exit -eq 0){$state.failures=0;$state.nextAttemptUtc='';$state.reason='Agent exited 0';Write-Log 'Codex continuation exited 0.'}
        else{
            $state.failures++;$delay=Reset-Delay $run.Output;$state.nextAttemptUtc=(Get-Date).ToUniversalTime().AddSeconds($delay).ToString('o');$state.reason="Codex exit $($run.Exit), consecutive failure $($state.failures)"
            Write-Log ($state.reason+'; next attempt no earlier than '+$state.nextAttemptUtc+'. Error output is retained in the redacted last-output file.')
            if($state.failures -ge 3){$state.halted=$true;Save-State $state;Write-Log 'STOP: three failed relaunches. Halt persists across scheduled starts; inspect the error before manually resetting state.';break}
        }
        Save-State $state;if($Once){break};Wait-Checkpoint 60
    }
    if(Test-Path -LiteralPath $complete){Write-Log 'TASK_COMPLETE.flag detected; watchdog stopped.'}
}catch{Write-Log ('ERROR: '+$_.Exception.Message);throw}finally{if($handle){$handle.Dispose()}}
