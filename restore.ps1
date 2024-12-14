<#
.NOTES
241107
#>
param (
    [Parameter(Mandatory, Position=0)]
    [string]$Project,

    [string]$ForceUseDotnetCLI,

    # [Parameter(Mandatory)]
    [string]$NugetConfigFile,

    [string]$PackagesDir,

    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
trap { throw $_ }

$env:DOTNET_CLI_UI_LANGUAGE = "en-US"
[CultureInfo]::DefaultThreadCurrentUICulture = [CultureInfo]::InvariantCulture

if ([string]::IsNullOrEmpty($Project)) {
    $Project = "."
}

if (Test-Path -PathType Container $Project) {
    $ProjectDir = $Project

    # $Project = Resolve-Path -ErrorAction Stop -Path (Join-Path $ProjectDir "*.*proj")
    # if ($Project.Count -ne 1) {
    #     throw "Multiple projects found: $Project"
    # }
    # $Project = $Project.Name
    $Project = "."
}
else {
    $ProjectDir = [IO.Path]::GetDirectoryName($Project)
    $Project = [IO.Path]::GetFileName($Project)
}

if ([string]::IsNullOrEmpty($NugetConfigFile)) {
    $dir = Get-Item -LiteralPath $ProjectDir
    while ($dir -ne $null) {
        $NugetConfigFile = Join-Path $dir.FullName "nuget.config"
        if (Test-Path -LiteralPath $NugetConfigFile) {
            break
        }
        $dir = $dir.Parent
    }
}
else {
    $NugetConfigFile = Resolve-Path -LiteralPath $NugetConfigFile
}

Push-Location $ProjectDir
try {
    if (-not $ForceUseDotnetCLI -and (Test-Path ".\packages.config")) {
        if ([string]::IsNullOrEmpty($PackagesDir)) {
            $SolutionDir = [IO.Path]::GetDirectoryName($NugetConfigFile)
            $PackagesDir = Join-Path $SolutionDir.FullName "packages"
        }

        $exeArgs = @(
            "-ConfigFile", $NugetConfigFile,
            # "-SolutionDirectory", $SolutionDir,
            "-PackagesDirectory", $PackagesDir,
            "-ForceEnglishOutput"
        )

        Write-Host "nuget restore $Project $exeArgs"
        & nuget restore $Project @exeArgs
        if ($LASTEXITCODE -ne 0) {
            Write-Error "nuget exited with $LASTEXITCODE"
        }
    }
    else {
        $exeArgs = @(
            "--interactive",
            "--configfile", $NugetConfigFile,
            "--verbosity", "normal",
            ""
        )

        if ([string]::IsNullOrEmpty($PackagesDir)) {
            # restore into "$(UserProfile)\.nuget\packages\" by default
        }
        else {
            $exeArgs += @(
                "--packages", $PackagesDir
            )
        }

        Write-Host "dotnet restore $Project $exeArgs"
        & dotnet restore $Project @exeArgs
        if ($LASTEXITCODE -ne 0) {
            Write-Error "dotnet exited with $LASTEXITCODE"
        }
    }
}
finally {
    Pop-Location
}
