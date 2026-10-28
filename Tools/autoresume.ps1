# Ilaban project auto-resume watchdog. One instance owns the lock for its lifetime.
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$tools = $PSScriptRoot
$lockPath = Join-Path $tools 'autoresume.lock'
$logPath = Join-Path $tools 'autoresume.log'
$outputPath = Join-Path $tools 'autoresume-last-output.txt'
$activeMarker = Join-Path $tools 'codex-active.lock'
$complete = Join-Path $repo 'TASK_COMPLETE.flag'
$codex = (Get-Command codex.exe -ErrorAction Stop).Source
$mutex = $null
function Write-Log([string]$message) {
    Add-Content -LiteralPath $logPath -Value "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz') $message" -Encoding UTF8
}
try {
    $mutex = [System.IO.File]::Open($lockPath, [System.IO.FileMode]::OpenOrCreate, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
} catch {
    exit 0
}
try {
    $bytes = [Text.Encoding]::UTF8.GetBytes("PID=$PID started=$(Get-Date -Format o)")
    $mutex.SetLength(0); $mutex.Write($bytes,0,$bytes.Length); $mutex.Flush()
    Write-Log "Watchdog started. Repo=$repo; Codex=$codex"
    $failures = 0
    $resumeInstruction = 'Open this project folder, read RESUME_STATE.txt, HANDOFF.md, PENDING_NETWORK.md and CHECKLIST.md, run git status, finish any half-done step, then continue the original plan from the resume command. Do not ask questions. Do not start the queued final exe/Drive task unless the pre-check passes.'
    while (-not (Test-Path -LiteralPath $complete)) {
        if (Test-Path -LiteralPath $activeMarker) {
            Write-Log 'Active-agent marker exists; skipping relaunch.'
            Start-Sleep -Seconds 60
            continue
        }
        $repoPattern = [regex]::Escape($repo)
        $alreadyWorking = Get-CimInstance Win32_Process -ErrorAction SilentlyContinue | Where-Object {
            $_.Name -match '^codex(\.exe)?$' -and $_.CommandLine -and $_.CommandLine -match $repoPattern
        } | Select-Object -First 1
        if ($alreadyWorking) {
            Write-Log "Codex process $($alreadyWorking.ProcessId) already references this repo; skipping."
            Start-Sleep -Seconds 60
            continue
        }
        $net = Test-NetConnection github.com -Port 443 -WarningAction SilentlyContinue
        if (-not $net.TcpTestSucceeded) {
            Write-Log 'Offline: github.com:443 unavailable; no Codex relaunch. Will check again in 60 seconds.'
            Start-Sleep -Seconds 60
            continue
        }
        Remove-Item -LiteralPath $outputPath -Force -ErrorAction SilentlyContinue
        Write-Log 'Launching codex exec with the checkpoint resume instruction.'
        & $codex exec --sandbox workspace-write -C $repo $resumeInstruction 2>&1 | Tee-Object -LiteralPath $outputPath
        $codexExit = $LASTEXITCODE
        $agentOutput = if (Test-Path -LiteralPath $outputPath) { Get-Content -LiteralPath $outputPath -Raw -ErrorAction SilentlyContinue } else { '' }
        if ($codexExit -eq 0) {
            $failures = 0
            Write-Log 'Codex process exited successfully.'
        } else {
            $failures++
            Write-Log "Codex relaunch failed with exit code $codexExit; consecutive failures=$failures."
            if ($failures -ge 3) { Write-Log 'Stopping after three failed relaunches in a row.'; break }
            $pause = 900
            $match = [regex]::Match($agentOutput, 'try again at\s+(?<time>\d{1,2}:\d{2}\s*[AP]M)', 'IgnoreCase')
            if ($match.Success) {
                try {
                    $when = [datetime]::ParseExact($match.Groups['time'].Value.Trim(), @('h:mm tt','hh:mm tt'), [Globalization.CultureInfo]::InvariantCulture)
                    $retryAt = (Get-Date).Date.Add($when.TimeOfDay)
                    if ($retryAt -le (Get-Date)) { $retryAt = $retryAt.AddDays(1) }
                    $pause = [Math]::Max(60,[int]($retryAt.AddMinutes(2)-(Get-Date)).TotalSeconds)
                    Write-Log "Rate-limit reset detected; waiting until $($retryAt.AddMinutes(2).ToString('o'))."
                } catch { Write-Log 'Could not parse reset time; using the 15-minute fallback.' }
            } else { Write-Log 'No readable reset time; using the 15-minute fallback.' }
            Start-Sleep -Seconds $pause
        }
        if (Test-Path -LiteralPath $complete) { break }
        Start-Sleep -Seconds 60
    }
    if (Test-Path -LiteralPath $complete) { Write-Log 'TASK_COMPLETE.flag detected; watchdog stopping.' }
} catch {
    Write-Log "ERROR: $($_.Exception.Message)"
    throw
} finally {
    if ($mutex) { $mutex.Dispose() }
}
