
function CompareDateTime {
    param (
        [Parameter(Mandatory)]
        [DateTime]$a,

        [Parameter(Mandatory)]
        [DateTime]$b
    )

    $sa = $a.ToString('yyyy-MM-ddTHH:mm:sszzz')
    $sb = $b.ToString('yyyy-MM-ddTHH:mm:sszzz')
    return $sa -eq $sb
}

function CompareDateTime {
    param (
        [Parameter(Mandatory)]
        [DateTime]$a,

        [Parameter(Mandatory)]
        [DateTime]$b
    )

    $ia = [uint64][Math]::Floor($a.Ticks / 10000000)
    $ib = [uint64][Math]::Floor($b.Ticks / 10000000)
    return $ia -eq $ib
}
