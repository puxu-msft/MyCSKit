<#
https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_comment_based_help

.SYNOPSIS
Alternative Start-Process

.DESCRIPTION
Neither pwsh Start-Process nor .NET Process provides the full features of Win32 API CreateProcess.

The key we need is flag "CREATE_NEW_PROCESS_GROUP" that allows process to listen "Ctrl+C" independently.

#>
param(
    [Parameter(Mandatory, Position=0)]
    [string]$Exe,

    [int]$waitInSec,

    # Makes PowerShell happy - last param has no trailing ","
    [switch]$dummy
)

$ErrorActionPreference = 'Stop'
trap { throw $_ }

$csFile = Resolve-Path -Path "$PSScriptRoot/winapi.cs"
Add-Type -TypeDefinition $(Get-Content $csFile -Raw) | Out-Null

Push-Location $dir
try {
    $exe = Resolve-Path ".\OnlineStoreServer.exe"
    $exeArgs = @($exe)

    if ($waitInSec -gt 0) {
        $uuid = "{{{0}}}" -f $(New-Guid).ToString()
        $script:waitEventHandle = [My.Spec]::CreateEvent($uuid)
        $exeArgs += @("/WaitEvent", $uuid)
    }

    $procinfo = [My.WinApi+PROCESS_INFORMATION]::new();

    Write-Host "start $exeArgs"
    $exeArgs = "{0}" -f [string]::Join(" ", $exeArgs)
    $ret = [My.Spec]::CreateProcess($exeArgs, [ref]$procinfo);
    if (-not $ret) {
        $err = [System.Runtime.InteropServices.Marshal]::GetLastWin32Error();
        Write-Error "Failed to CreateProcess, err=$err"
        exit 1
    }

    if ($waitInSec -gt 0) {
        $ret = [My.Spec]::WaitFor($procinfo.hProcess, $script:waitEventHandle, 1000 * $waitInSec);
        if (0 -ne $ret) {
            Write-Error "Failed to WaitFor, ret=$("{0:X8}" -f $ret)"
            exit 1
        }
    }
    Write-Host "started"
}
finally {
    Pop-Location
}

Start-Sleep -Seconds 0.2 | Out-Null
