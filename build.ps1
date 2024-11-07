<#
.NOTES
241107
.LINK
https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference?view=vs-2022
#>
param (
    [Parameter(Mandatory, Position=0)]
    [string]$Project,

    [string]$Target,
    [switch]$ReleaseBuild,

    [switch]$x64,

    [switch]$Clean,
    [switch]$V,
    [switch]$VV
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

if ('' -eq [string]$Target) {
    $Target = "Build"
}
$Targets = $Target.Split(',')
if ($Clean) {
    $Targets = @("Clean") + $Targets
}

$BuildProfile = @{
    Target = [string]::Join(',', $Targets);
    Configuration = if ($ReleaseBuild) {"Release"} else {"Debug"};
    Platform = if ($x64) {"x64"} else {"Any CPU"};
    MaxCpuCount = 4;
}

# see https://github.com/dotnet/msbuild/issues/1596
$env:DOTNET_CLI_UI_LANGUAGE = "en-US"
$env:PreferredUILang = "en-US"
$env:VSLANG = "1033"
# chcp 65001

Push-Location $ProjectDir
try {
    $exeArgs = @(
        $ProjectName,
        "-t:$($BuildProfile.Target)",
        "-p:Configuration=$($BuildProfile.Configuration)",
        "-p:Platform=$($BuildProfile.Platform)",
        "-MaxCpuCount:$($BuildProfile.MaxCpuCount)",
        "-NoLogo"
    )

    if ($VV) {
        $exeArgs += @("-verbosity:diag")
    }
    elseif ($V) {
        $exeArgs += @("-verbosity:detailed")
    }

    & dotnet msbuild @exeArgs
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet exited with code $LASTEXITCODE"
    }
}
finally {
    Pop-Location
}
