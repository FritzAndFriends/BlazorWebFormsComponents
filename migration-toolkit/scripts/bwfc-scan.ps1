<#
.SYNOPSIS
    Scans a Web Forms project for migration readiness to BlazorWebFormsComponents.

.DESCRIPTION
    Inventories all .aspx, .ascx, and .master files in a Web Forms project directory.
    For each file, extracts asp: control usage, code-behind references, data binding
    expressions, and DataSource controls. Produces a summary report with a migration
    readiness score based on BlazorWebFormsComponents (BWFC) coverage.

.PARAMETER Path
    Path to the Web Forms project root directory to scan.

.PARAMETER OutputFormat
    Format of the report output. Valid values are "Console", "Json", or "Markdown".
    Defaults to "Console".

.PARAMETER OutputFile
    Optional file path to write the report. If omitted, output is written to the
    console (or returned as a string for Json/Markdown when no file is specified).

.EXAMPLE
    .\bwfc-scan.ps1 -Path "C:\Projects\MyWebFormsApp"

    Scans the project and displays a colored console report.

.EXAMPLE
    .\bwfc-scan.ps1 -Path "C:\Projects\MyWebFormsApp" -OutputFormat Json -OutputFile "report.json"

    Scans the project and writes a JSON report to report.json.

.EXAMPLE
    .\bwfc-scan.ps1 -Path ".\src\WebApp" -OutputFormat Markdown -OutputFile "migration-report.md"

    Scans the project and writes a Markdown migration report.

.NOTES
    Part of the BlazorWebFormsComponents project.
    https://github.com/AzimoLabs/BlazorWebFormsComponents
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0, HelpMessage = "Path to the Web Forms project root directory.")]
    [ValidateNotNullOrEmpty()]
    [string]$Path,

    [Parameter(Mandatory = $false, HelpMessage = "Output format: Console, Json, or Markdown.")]
    [ValidateSet("Console", "Json", "Markdown")]
    [string]$OutputFormat = "Console",

    [Parameter(Mandatory = $false, HelpMessage = "Path to write the output file.")]
    [string]$OutputFile
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

#region Coverage Maps

$SupportedControls = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
@(
    "AdRotator", "BoundField", "BulletedList", "Button", "ButtonField", "Calendar",
    "CheckBox", "CheckBoxField", "CheckBoxList", "ChangePassword", "CommandField",
    "CompareValidator", "Content", "ContentPlaceHolder", "CreateUserWizard",
    "CustomValidator", "DataGrid", "DataList", "DataPager", "DetailsView",
    "DropDownList", "FileUpload", "FormView", "GridView", "HiddenField",
    "HyperLink", "HyperLinkField", "Image", "ImageButton", "ImageField",
    "Label", "LinkButton", "ListBox", "ListItem", "ListView", "Literal",
    "Localize", "Login", "LoginName", "LoginStatus", "LoginView", "Menu",
    "MenuItem", "ModelErrorMessage", "MultiView", "Panel", "PasswordRecovery",
    "PlaceHolder", "RadioButton", "RadioButtonList", "RangeValidator",
    "RegularExpressionValidator", "Repeater", "RequiredFieldValidator",
    "ScriptManager", "ScriptManagerProxy", "ScriptReference", "SiteMapPath",
    "Table", "TableCell", "TableHeaderCell", "TableHeaderRow", "TableRow",
    "TemplateField", "TextBox", "Timer", "TreeNode", "TreeView",
    "UpdatePanel", "UpdateProgress", "ValidationSummary", "View"
) | ForEach-Object { [void]$SupportedControls.Add($_) }

$UnsupportedControls = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
@(
    "SqlDataSource", "ObjectDataSource", "EntityDataSource", "LinqDataSource",
    "XmlDataSource", "SiteMapDataSource", "Wizard", "Xml", "Substitution"
) | ForEach-Object { [void]$UnsupportedControls.Add($_) }

$DataSourceControls = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
@(
    "SqlDataSource", "ObjectDataSource", "EntityDataSource", "LinqDataSource",
    "XmlDataSource", "SiteMapDataSource", "AccessDataSource"
) | ForEach-Object { [void]$DataSourceControls.Add($_) }

#endregion

#region Resolve and Validate Path

$ResolvedPath = $null
try {
    $ResolvedPath = (Resolve-Path -Path $Path -ErrorAction Stop).Path
}
catch {
    Write-Error "The specified path does not exist: $Path"
    return
}

if (-not (Test-Path -Path $ResolvedPath -PathType Container)) {
    Write-Error "The specified path is not a directory: $ResolvedPath"
    return
}

#endregion

#region Scan Files

$Extensions = @("*.aspx", "*.ascx", "*.master")
$AllFiles = @()
foreach ($ext in $Extensions) {
    $AllFiles += Get-ChildItem -Path $ResolvedPath -Filter $ext -Recurse -File -ErrorAction SilentlyContinue
}

if ($AllFiles.Count -eq 0) {
    Write-Warning "No .aspx, .ascx, or .master files found in: $ResolvedPath"
    return
}

# Per-file results
$FileResults = [System.Collections.Generic.List[PSObject]]::new()

# Global control inventory: control name -> total count
$GlobalControlCounts = @{}

# Patterns
$ControlRegex = [regex]::new('<asp:(\w+)', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
$CodeBehindRegex = [regex]::new('(?:CodeBehind|CodeFile)\s*=\s*"', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
$DataBindRegex = [regex]::new('<%#', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
$ViewStateRegex = [regex]::new('ViewState\s*\[', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
$SessionRegex = [regex]::new('Session\s*\[', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)

foreach ($file in $AllFiles) {
    try {
        $content = [System.IO.File]::ReadAllText($file.FullName)
    }
    catch {
        Write-Warning "Could not read file: $($file.FullName) - $_"
        continue
    }

    $relativePath = $file.FullName
    if ($file.FullName.StartsWith($ResolvedPath, [System.StringComparison]::OrdinalIgnoreCase)) {
        $relativePath = $file.FullName.Substring($ResolvedPath.Length).TrimStart('\', '/')
    }

    # Extract controls
    $controlCounts = @{}
    $matches = $ControlRegex.Matches($content)
    foreach ($m in $matches) {
        $controlName = $m.Groups[1].Value
        if ($controlCounts.ContainsKey($controlName)) {
            $controlCounts[$controlName]++
        }
        else {
            $controlCounts[$controlName] = 1
        }
        if ($GlobalControlCounts.ContainsKey($controlName)) {
            $GlobalControlCounts[$controlName]++
        }
        else {
            $GlobalControlCounts[$controlName] = 1
        }
    }

    # Detect patterns
    $hasCodeBehind = $CodeBehindRegex.IsMatch($content)
    $hasDataBinding = $DataBindRegex.IsMatch($content)
    $hasViewState = $ViewStateRegex.IsMatch($content)
    $hasSession = $SessionRegex.IsMatch($content)

    $usesDataSourceControls = $false
    foreach ($ctrl in $controlCounts.Keys) {
        if ($DataSourceControls.Contains($ctrl)) {
            $usesDataSourceControls = $true
            break
        }
    }

    $fileResult = [PSCustomObject]@{
        RelativePath          = $relativePath
        Extension             = $file.Extension.ToLower()
        Controls              = $controlCounts
        TotalControlCount     = ($controlCounts.Values | Measure-Object -Sum).Sum
        HasCodeBehind         = $hasCodeBehind
        HasDataBinding        = $hasDataBinding
        HasViewState          = $hasViewState
        HasSession            = $hasSession
        UsesDataSourceControls = $usesDataSourceControls
    }

    $FileResults.Add($fileResult)
}

#endregion

#region Build Summary

$aspxCount = ($FileResults | Where-Object { $_.Extension -eq ".aspx" }).Count
$ascxCount = ($FileResults | Where-Object { $_.Extension -eq ".ascx" }).Count
$masterCount = ($FileResults | Where-Object { $_.Extension -eq ".master" }).Count
$totalFiles = $FileResults.Count

# Build control inventory with status
$ControlInventory = [System.Collections.Generic.List[PSObject]]::new()
$coveredUsageCount = 0
$totalUsageCount = 0

foreach ($entry in $GlobalControlCounts.GetEnumerator() | Sort-Object -Property Key) {
    $controlName = $entry.Key
    $count = $entry.Value
    $totalUsageCount += $count

    if ($SupportedControls.Contains($controlName)) {
        $status = "Supported"
        $coveredUsageCount += $count
    }
    elseif ($UnsupportedControls.Contains($controlName)) {
        $status = "NotSupported"
    }
    else {
        $status = "Unknown"
    }

    $ControlInventory.Add([PSCustomObject]@{
        Control = $controlName
        Count   = $count
        Status  = $status
    })
}

# Migration readiness score
$readinessScore = 0
if ($totalUsageCount -gt 0) {
    $readinessScore = [math]::Round(($coveredUsageCount / $totalUsageCount) * 100, 1)
}

# Files needing special attention
$DataSourceFiles = $FileResults | Where-Object { $_.UsesDataSourceControls }
$ViewStateFiles = $FileResults | Where-Object { $_.HasViewState }
$SessionFiles = $FileResults | Where-Object { $_.HasSession }

$Summary = [PSCustomObject]@{
    ProjectPath       = $ResolvedPath
    TotalFiles        = $totalFiles
    AspxFiles         = $aspxCount
    AscxFiles         = $ascxCount
    MasterFiles       = $masterCount
    ControlInventory  = $ControlInventory
    TotalControlUsage = $totalUsageCount
    CoveredUsage      = $coveredUsageCount
    ReadinessScore    = $readinessScore
    FileDetails       = $FileResults
    DataSourceFiles   = @($DataSourceFiles | ForEach-Object { $_.RelativePath })
    ViewStateFiles    = @($ViewStateFiles | ForEach-Object { $_.RelativePath })
    SessionFiles      = @($SessionFiles | ForEach-Object { $_.RelativePath })
}

#endregion

#region Output Functions

function Write-ConsoleReport {
    param([PSCustomObject]$Report)

    Write-Host ""
    Write-Host "=====================================================================" -ForegroundColor Cyan
    Write-Host "  BlazorWebFormsComponents - Migration Readiness Report" -ForegroundColor Cyan
    Write-Host "=====================================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  Project: $($Report.ProjectPath)" -ForegroundColor White
    Write-Host ""

    # File summary
    Write-Host "  FILES SCANNED" -ForegroundColor Yellow
    Write-Host "  ─────────────────────────────────────────" -ForegroundColor DarkGray
    Write-Host "    .aspx  files:  $($Report.AspxFiles)" -ForegroundColor White
    Write-Host "    .ascx  files:  $($Report.AscxFiles)" -ForegroundColor White
    Write-Host "    .master files: $($Report.MasterFiles)" -ForegroundColor White
    Write-Host "    Total:         $($Report.TotalFiles)" -ForegroundColor White
    Write-Host ""

    # Control inventory
    Write-Host "  CONTROL INVENTORY" -ForegroundColor Yellow
    Write-Host "  ─────────────────────────────────────────" -ForegroundColor DarkGray
    Write-Host ("    {0,-30} {1,6}  {2}" -f "Control", "Count", "BWFC Status") -ForegroundColor DarkGray
    Write-Host "    $("-" * 55)" -ForegroundColor DarkGray

    foreach ($item in $Report.ControlInventory) {
        $icon = switch ($item.Status) {
            "Supported"    { "`u{2705}" }
            "NotSupported" { "`u{274C}" }
            default        { "`u{2753}" }
        }
        $color = switch ($item.Status) {
            "Supported"    { "Green" }
            "NotSupported" { "Red" }
            default        { "DarkYellow" }
        }
        $statusLabel = switch ($item.Status) {
            "Supported"    { "Supported" }
            "NotSupported" { "Not Supported" }
            default        { "Unknown" }
        }
        Write-Host ("    {0,-30} {1,6}  {2} {3}" -f $item.Control, $item.Count, $icon, $statusLabel) -ForegroundColor $color
    }
    Write-Host ""

    # Readiness score
    $scoreColor = if ($Report.ReadinessScore -ge 80) { "Green" }
                  elseif ($Report.ReadinessScore -ge 50) { "Yellow" }
                  else { "Red" }

    Write-Host "  MIGRATION READINESS" -ForegroundColor Yellow
    Write-Host "  ─────────────────────────────────────────" -ForegroundColor DarkGray
    Write-Host "    Controls with BWFC coverage: $($Report.CoveredUsage) / $($Report.TotalControlUsage)" -ForegroundColor White
    Write-Host "    Readiness Score: $($Report.ReadinessScore)%" -ForegroundColor $scoreColor
    Write-Host ""

    # Special attention
    $hasWarnings = ($Report.DataSourceFiles.Count -gt 0) -or
                   ($Report.ViewStateFiles.Count -gt 0) -or
                   ($Report.SessionFiles.Count -gt 0)

    if ($hasWarnings) {
        Write-Host "  FILES NEEDING SPECIAL ATTENTION" -ForegroundColor Yellow
        Write-Host "  ─────────────────────────────────────────" -ForegroundColor DarkGray

        if ($Report.DataSourceFiles.Count -gt 0) {
            Write-Host "    DataSource Controls ($($Report.DataSourceFiles.Count) files):" -ForegroundColor Red
            foreach ($f in $Report.DataSourceFiles) {
                Write-Host "      - $f" -ForegroundColor DarkYellow
            }
        }

        if ($Report.ViewStateFiles.Count -gt 0) {
            Write-Host "    ViewState Usage ($($Report.ViewStateFiles.Count) files):" -ForegroundColor Red
            foreach ($f in $Report.ViewStateFiles) {
                Write-Host "      - $f" -ForegroundColor DarkYellow
            }
        }

        if ($Report.SessionFiles.Count -gt 0) {
            Write-Host "    Session Usage ($($Report.SessionFiles.Count) files):" -ForegroundColor Red
            foreach ($f in $Report.SessionFiles) {
                Write-Host "      - $f" -ForegroundColor DarkYellow
            }
        }
        Write-Host ""
    }

    Write-Host "=====================================================================" -ForegroundColor Cyan
    Write-Host ""
}

function ConvertTo-JsonReport {
    param([PSCustomObject]$Report)

    $jsonObj = [ordered]@{
        projectPath      = $Report.ProjectPath
        summary          = [ordered]@{
            totalFiles   = $Report.TotalFiles
            aspxFiles    = $Report.AspxFiles
            ascxFiles    = $Report.AscxFiles
            masterFiles  = $Report.MasterFiles
        }
        readiness        = [ordered]@{
            score             = $Report.ReadinessScore
            coveredUsage      = $Report.CoveredUsage
            totalControlUsage = $Report.TotalControlUsage
        }
        controlInventory = @($Report.ControlInventory | ForEach-Object {
            [ordered]@{
                control = $_.Control
                count   = $_.Count
                status  = $_.Status
            }
        })
        specialAttention = [ordered]@{
            dataSourceFiles = $Report.DataSourceFiles
            viewStateFiles  = $Report.ViewStateFiles
            sessionFiles    = $Report.SessionFiles
        }
        fileDetails      = @($Report.FileDetails | ForEach-Object {
            [ordered]@{
                path                   = $_.RelativePath
                extension              = $_.Extension
                totalControls          = $_.TotalControlCount
                controls               = $_.Controls
                hasCodeBehind          = $_.HasCodeBehind
                hasDataBinding         = $_.HasDataBinding
                hasViewState           = $_.HasViewState
                hasSession             = $_.HasSession
                usesDataSourceControls = $_.UsesDataSourceControls
            }
        })
    }

    return $jsonObj | ConvertTo-Json -Depth 10
}

function ConvertTo-MarkdownReport {
    param([PSCustomObject]$Report)

    $sb = [System.Text.StringBuilder]::new()

    [void]$sb.AppendLine("# BlazorWebFormsComponents - Migration Readiness Report")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("**Project:** ``$($Report.ProjectPath)``")
    [void]$sb.AppendLine("")

    # File summary
    [void]$sb.AppendLine("## Files Scanned")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("| File Type | Count |")
    [void]$sb.AppendLine("|-----------|------:|")
    [void]$sb.AppendLine("| .aspx     | $($Report.AspxFiles) |")
    [void]$sb.AppendLine("| .ascx     | $($Report.AscxFiles) |")
    [void]$sb.AppendLine("| .master   | $($Report.MasterFiles) |")
    [void]$sb.AppendLine("| **Total** | **$($Report.TotalFiles)** |")
    [void]$sb.AppendLine("")

    # Readiness
    $scoreEmoji = if ($Report.ReadinessScore -ge 80) { "`u{1F7E2}" }
                  elseif ($Report.ReadinessScore -ge 50) { "`u{1F7E1}" }
                  else { "`u{1F534}" }

    [void]$sb.AppendLine("## Migration Readiness")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("$scoreEmoji **Score: $($Report.ReadinessScore)%** ($($Report.CoveredUsage) of $($Report.TotalControlUsage) control usages covered)")
    [void]$sb.AppendLine("")

    # Control inventory
    [void]$sb.AppendLine("## Control Inventory")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("| Control | Count | BWFC Status |")
    [void]$sb.AppendLine("|---------|------:|-------------|")
    foreach ($item in $Report.ControlInventory) {
        $statusText = switch ($item.Status) {
            "Supported"    { "`u{2705} Supported" }
            "NotSupported" { "`u{274C} Not Supported" }
            default        { "`u{2753} Unknown" }
        }
        [void]$sb.AppendLine("| $($item.Control) | $($item.Count) | $statusText |")
    }
    [void]$sb.AppendLine("")

    # Special attention
    $hasWarnings = ($Report.DataSourceFiles.Count -gt 0) -or
                   ($Report.ViewStateFiles.Count -gt 0) -or
                   ($Report.SessionFiles.Count -gt 0)

    if ($hasWarnings) {
        [void]$sb.AppendLine("## Files Needing Special Attention")
        [void]$sb.AppendLine("")

        if ($Report.DataSourceFiles.Count -gt 0) {
            [void]$sb.AppendLine("### DataSource Controls ($($Report.DataSourceFiles.Count) files)")
            [void]$sb.AppendLine("")
            foreach ($f in $Report.DataSourceFiles) {
                [void]$sb.AppendLine("- ``$f``")
            }
            [void]$sb.AppendLine("")
        }

        if ($Report.ViewStateFiles.Count -gt 0) {
            [void]$sb.AppendLine("### ViewState Usage ($($Report.ViewStateFiles.Count) files)")
            [void]$sb.AppendLine("")
            foreach ($f in $Report.ViewStateFiles) {
                [void]$sb.AppendLine("- ``$f``")
            }
            [void]$sb.AppendLine("")
        }

        if ($Report.SessionFiles.Count -gt 0) {
            [void]$sb.AppendLine("### Session Usage ($($Report.SessionFiles.Count) files)")
            [void]$sb.AppendLine("")
            foreach ($f in $Report.SessionFiles) {
                [void]$sb.AppendLine("- ``$f``")
            }
            [void]$sb.AppendLine("")
        }
    }

    # Per-file details
    [void]$sb.AppendLine("## File Details")
    [void]$sb.AppendLine("")
    foreach ($file in $Report.FileDetails | Sort-Object -Property RelativePath) {
        if ($file.TotalControlCount -eq 0) { continue }
        [void]$sb.AppendLine("### ``$($file.RelativePath)``")
        [void]$sb.AppendLine("")
        $flags = @()
        if ($file.HasCodeBehind)          { $flags += "Code-Behind" }
        if ($file.HasDataBinding)         { $flags += "Data Binding" }
        if ($file.HasViewState)           { $flags += "`u{26A0}`u{FE0F} ViewState" }
        if ($file.HasSession)             { $flags += "`u{26A0}`u{FE0F} Session" }
        if ($file.UsesDataSourceControls) { $flags += "`u{26A0}`u{FE0F} DataSource Controls" }
        if ($flags.Count -gt 0) {
            [void]$sb.AppendLine("**Flags:** $($flags -join ', ')")
            [void]$sb.AppendLine("")
        }
        [void]$sb.AppendLine("| Control | Count |")
        [void]$sb.AppendLine("|---------|------:|")
        foreach ($ctrl in $file.Controls.GetEnumerator() | Sort-Object -Property Key) {
            [void]$sb.AppendLine("| $($ctrl.Key) | $($ctrl.Value) |")
        }
        [void]$sb.AppendLine("")
    }

    return $sb.ToString()
}

#endregion

#region Emit Output

switch ($OutputFormat) {
    "Console" {
        Write-ConsoleReport -Report $Summary
        if ($OutputFile) {
            # For console format written to file, strip color by rewriting as plain text
            $plainLines = @()
            $plainLines += "====================================================================="
            $plainLines += "  BlazorWebFormsComponents - Migration Readiness Report"
            $plainLines += "====================================================================="
            $plainLines += ""
            $plainLines += "  Project: $($Summary.ProjectPath)"
            $plainLines += ""
            $plainLines += "  FILES SCANNED"
            $plainLines += "    .aspx  files:  $($Summary.AspxFiles)"
            $plainLines += "    .ascx  files:  $($Summary.AscxFiles)"
            $plainLines += "    .master files: $($Summary.MasterFiles)"
            $plainLines += "    Total:         $($Summary.TotalFiles)"
            $plainLines += ""
            $plainLines += "  CONTROL INVENTORY"
            $plainLines += ("    {0,-30} {1,6}  {2}" -f "Control", "Count", "BWFC Status")
            foreach ($item in $Summary.ControlInventory) {
                $icon = switch ($item.Status) {
                    "Supported"    { "[OK]" }
                    "NotSupported" { "[NO]" }
                    default        { "[??]" }
                }
                $statusLabel = switch ($item.Status) {
                    "Supported"    { "Supported" }
                    "NotSupported" { "Not Supported" }
                    default        { "Unknown" }
                }
                $plainLines += ("    {0,-30} {1,6}  {2} {3}" -f $item.Control, $item.Count, $icon, $statusLabel)
            }
            $plainLines += ""
            $plainLines += "  MIGRATION READINESS"
            $plainLines += "    Controls with BWFC coverage: $($Summary.CoveredUsage) / $($Summary.TotalControlUsage)"
            $plainLines += "    Readiness Score: $($Summary.ReadinessScore)%"
            $plainLines += ""
            $plainLines += "====================================================================="

            $plainLines | Out-File -FilePath $OutputFile -Encoding utf8
            Write-Host "Report written to: $OutputFile" -ForegroundColor Green
        }
    }
    "Json" {
        $json = ConvertTo-JsonReport -Report $Summary
        if ($OutputFile) {
            $json | Out-File -FilePath $OutputFile -Encoding utf8
            Write-Host "JSON report written to: $OutputFile" -ForegroundColor Green
        }
        else {
            Write-Output $json
        }
    }
    "Markdown" {
        $md = ConvertTo-MarkdownReport -Report $Summary
        if ($OutputFile) {
            $md | Out-File -FilePath $OutputFile -Encoding utf8
            Write-Host "Markdown report written to: $OutputFile" -ForegroundColor Green
        }
        else {
            Write-Output $md
        }
    }
}

#endregion
