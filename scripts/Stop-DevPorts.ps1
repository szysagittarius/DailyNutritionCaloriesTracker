<#
.SYNOPSIS
    Frees the ports used by Nutrition Tracker before (re)starting dev services.

.DESCRIPTION
    Scans each well-known project port, shows what process is holding it,
    and optionally kills those processes so the dev services can bind cleanly.

.PARAMETER Ports
    Override the default port list.  Defaults to all project ports.

.PARAMETER WhatIf
    Show which processes would be killed without actually killing them.

.EXAMPLE
    # Free all dev ports, prompt before each kill
    .\scripts\Stop-DevPorts.ps1

.EXAMPLE
    # Preview only — no processes are killed
    .\scripts\Stop-DevPorts.ps1 -WhatIf

.EXAMPLE
    # Free only the API ports
    .\scripts\Stop-DevPorts.ps1 -Ports 7155,5155
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [int[]] $Ports = @(7155, 5155, 5173, 5174, 5175, 7071, 10002)
)

# ── Port metadata ────────────────────────────────────────────────────────────
$portMeta = @{
    7155  = 'REST API  - HTTPS  (dotnet run --launch-profile https)'
    5155  = 'REST API  - HTTP   (dotnet run --launch-profile https or http)'
    5173  = 'Frontend  - Vite   (npm run dev)'
    5174  = 'Frontend  - Vite   (npm run dev, fallback)'
    5175  = 'Frontend  - Vite   (npm run dev, fallback)'
    7071  = 'Azure Functions    (func start --port 7071)'
    10002 = 'Azurite Tables     (azurite --tablePort 10002)'
}

# ── Helpers ──────────────────────────────────────────────────────────────────
function Get-PortOwner([int]$Port) {
    # netstat outputs lines like: TCP  0.0.0.0:7155  ...  LISTENING  1234
    $lines = netstat -ano 2>$null |
        Select-String -Pattern "\s+(?:TCP|UDP)\s+[0-9.*]+:$Port\s" |
        Where-Object { $_ -match 'LISTEN|ESTABLISHED|UDP' }

    $pids = $lines |
        ForEach-Object { ($_ -split '\s+')[-1] } |
        Where-Object { $_ -match '^\d+$' } |
        Select-Object -Unique

    foreach ($p in $pids) {
        try   { Get-Process -Id ([int]$p) -ErrorAction Stop }
        catch { }
    }
}

# ── Main scan ────────────────────────────────────────────────────────────────
$found = [System.Collections.Generic.List[object]]::new()

Write-Host ''
Write-Host '  Nutrition Tracker - Port Scan' -ForegroundColor Cyan
    Write-Host ('  ' + ('-' * 70)) -ForegroundColor DarkGray
    Write-Host ('{0,-7} {1,-45} {2,-10} {3}' -f 'Port', 'Service', 'PID', 'Process') -ForegroundColor White
    Write-Host ('  ' + ('-' * 70)) -ForegroundColor DarkGray

foreach ($port in ($Ports | Sort-Object)) {
    $label    = if ($portMeta.ContainsKey($port)) { $portMeta[$port] } else { '(custom)' }
    $procs    = Get-PortOwner $port

    if ($procs) {
        foreach ($proc in $procs) {
            Write-Host ('{0,-7} {1,-45} {2,-10} {3}' -f $port, $label, $proc.Id, $proc.ProcessName) `
                -ForegroundColor Yellow
            $found.Add([pscustomobject]@{ Port = $port; Process = $proc })
        }
    } else {
        Write-Host ('{0,-7} {1,-45} {2}' -f $port, $label, 'free') -ForegroundColor DarkGreen
    }
}

Write-Host ('  ' + ('-' * 70)) -ForegroundColor DarkGray
Write-Host ''

# -- Kill prompt ───────────────────────────────────────────────────────────────
if ($found.Count -eq 0) {
    Write-Host '  All ports are free. Ready to start dev services.' -ForegroundColor Green
    Write-Host ''
    exit 0
}

Write-Host "  $($found.Count) port(s) in use." -ForegroundColor Yellow

if ($WhatIfPreference) {
    Write-Host '  WhatIf: no processes were killed.' -ForegroundColor Cyan
    Write-Host ''
    exit 0
}

$answer = Read-Host '  Kill all listed processes? [Y/n]'
if ($answer -match '^[Yy]$|^$') {
    $killed = 0
    foreach ($item in $found) {
        $proc = $item.Process
        if ($PSCmdlet.ShouldProcess("$($proc.ProcessName) (PID $($proc.Id)) on port $($item.Port)", 'Stop-Process')) {
            try {
                Stop-Process -Id $proc.Id -Force -ErrorAction Stop
                Write-Host "  Killed $($proc.ProcessName) (PID $($proc.Id))" -ForegroundColor Green
                $killed++
            } catch {
                Write-Warning "  Could not kill PID $($proc.Id): $_"
            }
        }
    }
    Write-Host ''
    Write-Host "  Done. $killed process(es) stopped." -ForegroundColor Green
} else {
    Write-Host '  Aborted — no processes were killed.' -ForegroundColor DarkYellow
}

Write-Host ''
