$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$watchdog=Join-Path $PSScriptRoot 'autoresume.ps1'
$cli=(Get-Command codex.exe -ErrorAction Stop).Source
function Ac-Seconds([string]$group,[string]$setting){
    $output=& powercfg /query SCHEME_CURRENT $group $setting
    $match=[regex]::Match(($output -join "`n"),'Current AC Power Setting Index:\s+0x([0-9a-fA-F]+)')
    if(-not $match.Success){throw "Cannot read existing AC power setting $setting"}
    [Convert]::ToInt64($match.Groups[1].Value,16)
}
$beforeSleep=Ac-Seconds 'SUB_SLEEP' 'STANDBYIDLE';$beforeMonitor=Ac-Seconds 'SUB_VIDEO' 'VIDEOIDLE'
& powercfg /change standby-timeout-ac 0;if($LASTEXITCODE -ne 0){throw 'Could not set plugged-in standby timeout'}
& powercfg /change monitor-timeout-ac 0;if($LASTEXITCODE -ne 0){throw 'Could not set plugged-in monitor timeout'}
$registered=$false;$taskError=''
$arguments='-NoProfile -WindowStyle Hidden -File "'+$watchdog+'" -CodexPath "'+$cli+'"'
try{
    $action=New-ScheduledTaskAction -Execute 'powershell.exe' -Argument $arguments -WorkingDirectory $repo
    $user=[Security.Principal.WindowsIdentity]::GetCurrent().Name
    $logon=New-ScheduledTaskTrigger -AtLogOn -User $user
    $repeat=New-ScheduledTaskTrigger -Once -At (Get-Date).AddMinutes(1) -RepetitionInterval (New-TimeSpan -Minutes 5) -RepetitionDuration (New-TimeSpan -Days 3650)
    $settings=New-ScheduledTaskSettingsSet -StartWhenAvailable -ExecutionTimeLimit ([TimeSpan]::Zero) -MultipleInstances IgnoreNew -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
    $principal=New-ScheduledTaskPrincipal -UserId $user -LogonType Interactive -RunLevel Limited
    Register-ScheduledTask -TaskName 'IlabanAutoResume' -Action $action -Trigger @($logon,$repeat) -Settings $settings -Principal $principal -Force|Out-Null
    $registered=$null -ne (Get-ScheduledTask -TaskName 'IlabanAutoResume' -ErrorAction Stop)
}catch{$taskError=$_.Exception.Message;Write-Warning "Scheduled task registration failed: $taskError"}
$existing=@(Get-CimInstance Win32_Process -Filter "Name='powershell.exe'"|Where-Object {$_.CommandLine -and $_.CommandLine -match [regex]::Escape($watchdog)})
if($existing.Count){$backgroundPid=$existing[0].ProcessId}else{$background=Start-Process -FilePath 'powershell.exe' -ArgumentList $arguments -WorkingDirectory $repo -WindowStyle Hidden -PassThru;$backgroundPid=$background.Id}
$report=[pscustomobject]@{utc=(Get-Date).ToUniversalTime().ToString('o');previous_ac_sleep_seconds=$beforeSleep;previous_ac_monitor_seconds=$beforeMonitor;current_ac_sleep_seconds=(Ac-Seconds 'SUB_SLEEP' 'STANDBYIDLE');current_ac_monitor_seconds=(Ac-Seconds 'SUB_VIDEO' 'VIDEOIDLE');task_registered=$registered;task_error=$taskError;watchdog_pid=$backgroundPid;scheduled_task='IlabanAutoResume';triggers=@('At logon for current user','Every 5 minutes');restore_sleep_command=('powercfg /change standby-timeout-ac '+($beforeSleep/60));restore_monitor_command=('powercfg /change monitor-timeout-ac '+($beforeMonitor/60))}
$report|ConvertTo-Json|Set-Content (Join-Path $repo 'Docs/QA_EVIDENCE/autoresume_setup.json') -Encoding UTF8
$report|ConvertTo-Json
