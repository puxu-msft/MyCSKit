
$ErrorActionPreference = 'stop'
trap { throw $Error[0]; }

$projs = @(
    # "$PSScriptRoot\Exe.CockpitAgent",
    # "$PSScriptRoot\Exe.ColumnKeyParser",
    # "$PSScriptRoot\Exe.NativeSortingParser",
    # "$PSScriptRoot\Exe.OSKey2PID",
    "$PSScriptRoot\Exe.RandomWriter",
    "$PSScriptRoot\Exe.SaasDocDataDumpWriter",
    # "$PSScriptRoot\Exe.Simple462",
    ""
)

foreach ($proj in $projs) {
    if ([string]::IsNullOrEmpty($proj)) {
        continue
    }

    # -PackagesDir "$PSScriptRoot\packages"
    & "$PSScriptRoot\restore.ps1" -ProjectDir $proj -NugetConfigFile "$PSScriptRoot\nuget.config"
}
