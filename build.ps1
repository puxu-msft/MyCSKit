<#
.NOTES
    241201

.LINK
    https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference?view=vs-2022
#>
[CmdletBinding()]
param (
    [Parameter(Mandatory, Position=0)]
    [string]$Project,

    [string]$Target,

    [switch]$ReBuild,
    [switch]$Clean,

    [switch]$ReleaseBuild,
    [switch]$Retail,

    [switch]$AnyCPU,

    [switch]$Restore,

    [switch]$V,
    [switch]$VV,

    [Parameter(ValueFromRemainingArguments)]
    $Remaining
)

$ErrorActionPreference = 'Stop'
trap { throw $_ }

$Project = Resolve-Path -LiteralPath $Project
if (Test-Path -PathType Container $Project) {
    $ProjectDir = $Project
    $ProjectName = "."
}
else {
    $ProjectDir = [IO.Path]::GetDirectoryName($Project)
    $ProjectName = [IO.Path]::GetFileName($Project)
}

function main {
    # see https://github.com/dotnet/msbuild/issues/1596
    $env:DOTNET_CLI_UI_LANGUAGE = "en-US"
    $env:PreferredUILang = "en-US"
    $env:VSLANG = "1033"
    # chcp 65001

    if ('' -eq [string]$Target) {
        $Target = "Build"
    }
    $Targets = $Target.Split(',')
    if ($ReBuild) {
        $Targets = @("Clean") + $Targets
    }
    elseif ($Clean) {
        $Targets = @("Clean")
    }

    if ($Retail) {
        $ReleaseBuild = $true
    }

    $Targets = [string]::Join(',', $Targets)
    $Configuration = if ($ReleaseBuild) {"Release"} else {"Debug"}
    $Platform = if ($AnyCPU) {"Any CPU"} else {"x64"}

    $exeArgs = @(
        $ProjectName
        "-t:$Targets"
        "-p:Configuration=$Configuration"
        "-p:Platform=$Platform"
        "-Restore:$Restore"
        "-NoLogo"
    )

    if ($VV) {
        $exeArgs += @("-verbosity:diag")
    }
    elseif ($V) {
        $exeArgs += @("-verbosity:detailed")
    }
    else {
        $exeArgs += @("-MaxCpuCount:4")
    }

    # "dotnet build" is for dotnet core only
    & dotnet msbuild @exeArgs
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet msbuild exited with code $LASTEXITCODE"
    }

}

Push-Location $ProjectDir
try {
    main
}
finally {
    Pop-Location
}
