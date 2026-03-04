<#
.SYNOPSIS
    Performs mechanical regex-based transforms on ASP.NET Web Forms files to produce Blazor-ready output.

.DESCRIPTION
    bwfc-migrate.ps1 is Layer 1 of the three-layer Web Forms to Blazor migration pipeline.
    It automates ~40% of the migration by applying safe, mechanical regex transforms:

      - Renames .aspx/.ascx/.master files to .razor
      - Strips Web Forms directives and converts to Razor equivalents
      - Removes asp: prefixes and runat="server" attributes
      - Converts Web Forms expressions to Razor syntax
      - Transforms URL references from ~/ to /
      - Cleans up Web Forms-specific attributes
      - Copies code-behind files with TODO annotations

    Semantic transforms (lifecycle methods, data binding logic, event handlers)
    are NOT handled here — those require the Copilot skill layer (Layer 2).

.PARAMETER Path
    Path to the Web Forms project root directory containing .aspx/.ascx/.master files.

.PARAMETER Output
    Path for the Blazor output project directory. Will be created if it does not exist.

.PARAMETER WhatIf
    Show what transforms would be applied without writing any files.

.PARAMETER Verbose
    Show detailed per-file transform log during processing.

.PARAMETER SkipProjectScaffold
    Skip generating .csproj, Program.cs, and _Imports.razor scaffold files.

.EXAMPLE
    .\bwfc-migrate.ps1 -Path C:\src\MyWebFormsApp -Output C:\src\MyBlazorApp

    Migrates all Web Forms files from MyWebFormsApp into a new Blazor project at MyBlazorApp.

.EXAMPLE
    .\bwfc-migrate.ps1 -Path C:\src\MyWebFormsApp -Output C:\src\MyBlazorApp -WhatIf

    Shows what transforms would be applied without creating any files.

.EXAMPLE
    .\bwfc-migrate.ps1 -Path .\LegacyApp -Output .\BlazorApp -SkipProjectScaffold -Verbose

    Transforms files with detailed logging, skipping project scaffold generation.
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory = $true, HelpMessage = "Path to the Web Forms project root")]
    [string]$Path,

    [Parameter(Mandatory = $true, HelpMessage = "Path for the Blazor output project")]
    [string]$Output,

    [Parameter(HelpMessage = "Skip creating .csproj, Program.cs, and _Imports.razor")]
    [switch]$SkipProjectScaffold
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

#region --- Configuration ---

$WebFormsExtensions = @('.aspx', '.ascx', '.master')
$CodeBehindExtensions = @('.aspx.cs', '.ascx.cs', '.master.cs', '.aspx.vb', '.ascx.vb', '.master.vb')
$StaticExtensions = @('.css', '.js', '.png', '.jpg', '.jpeg', '.gif', '.svg', '.ico', '.woff', '.woff2', '.ttf', '.eot')

# Attributes to strip completely (case-insensitive patterns)
$StripAttributes = @(
    'runat\s*=\s*"server"',
    'AutoEventWireup\s*=\s*"(true|false)"',
    'EnableViewState\s*=\s*"(true|false)"',
    'ViewStateMode\s*=\s*"[^"]*"',
    'ValidateRequest\s*=\s*"(true|false)"',
    'MaintainScrollPositionOnPostBack\s*=\s*"(true|false)"',
    'ClientIDMode\s*=\s*"[^"]*"'
)

#endregion

#region --- Logging & Tracking ---

$script:TransformLog = [System.Collections.Generic.List[PSCustomObject]]::new()
$script:ManualItems = [System.Collections.Generic.List[PSCustomObject]]::new()
$script:FilesProcessed = 0
$script:TransformsApplied = 0

function Write-TransformLog {
    param(
        [string]$File,
        [string]$Transform,
        [string]$Detail
    )
    $entry = [PSCustomObject]@{
        File      = $File
        Transform = $Transform
        Detail    = $Detail
    }
    $script:TransformLog.Add($entry)
    $script:TransformsApplied++
    if ($VerbosePreference -eq 'Continue') {
        Write-Verbose "  [$Transform] $Detail"
    }
}

function Write-ManualItem {
    param(
        [string]$File,
        [string]$Category,
        [string]$Detail
    )
    $entry = [PSCustomObject]@{
        File     = $File
        Category = $Category
        Detail   = $Detail
    }
    $script:ManualItems.Add($entry)
}

#endregion

#region --- Project Scaffolding ---

function New-ProjectScaffold {
    param(
        [string]$OutputRoot,
        [string]$ProjectName
    )

    # .csproj
    $csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Fritz.BlazorWebFormsComponents" Version="*" />
  </ItemGroup>

</Project>
"@

    # _Imports.razor
    $importsContent = @"
@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop
@using BlazorWebFormsComponents
@using $ProjectName
"@

    # Program.cs
    $programContent = @"
// TODO: Review and adjust this generated Program.cs for your application needs.
using BlazorWebFormsComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<$ProjectName.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
"@

    # Properties/launchSettings.json
    $launchSettingsContent = @"
{
  "profiles": {
    "$ProjectName": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
"@

    if ($PSCmdlet.ShouldProcess($OutputRoot, "Create project scaffold")) {
        $csprojPath = Join-Path $OutputRoot "$ProjectName.csproj"
        $importsPath = Join-Path $OutputRoot "_Imports.razor"
        $programPath = Join-Path $OutputRoot "Program.cs"
        $propertiesDir = Join-Path $OutputRoot "Properties"
        $launchSettingsPath = Join-Path $propertiesDir "launchSettings.json"

        Set-Content -Path $csprojPath -Value $csprojContent -Encoding UTF8
        Write-TransformLog -File $csprojPath -Transform 'Scaffold' -Detail "Generated $ProjectName.csproj"

        Set-Content -Path $importsPath -Value $importsContent -Encoding UTF8
        Write-TransformLog -File $importsPath -Transform 'Scaffold' -Detail 'Generated _Imports.razor'

        Set-Content -Path $programPath -Value $programContent -Encoding UTF8
        Write-TransformLog -File $programPath -Transform 'Scaffold' -Detail 'Generated Program.cs'

        New-Item -ItemType Directory -Force $propertiesDir | Out-Null
        Set-Content -Path $launchSettingsPath -Value $launchSettingsContent -Encoding UTF8
        Write-TransformLog -File $launchSettingsPath -Transform 'Scaffold' -Detail 'Generated Properties/launchSettings.json'
    }
    else {
        Write-Host "[WhatIf] Would create: $ProjectName.csproj"
        Write-Host "[WhatIf] Would create: _Imports.razor"
        Write-Host "[WhatIf] Would create: Program.cs"
        Write-Host "[WhatIf] Would create: Properties/launchSettings.json"
    }
}

function New-AppRazorScaffold {
    param(
        [string]$OutputRoot,
        [string]$ProjectName
    )

    $componentsDir = Join-Path $OutputRoot "Components"

    $appRazorContent = @"
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <HeadOutlet @rendermode="InteractiveServer" />
</head>

<body>
    <Routes @rendermode="InteractiveServer" />
    <script src="_framework/blazor.web.js"></script>
</body>

</html>
"@

    $routesRazorContent = @"
<Router AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
"@

    if ($PSCmdlet.ShouldProcess($componentsDir, "Create App.razor and Routes.razor scaffold")) {
        if (-not (Test-Path $componentsDir)) {
            New-Item -ItemType Directory -Force $componentsDir | Out-Null
        }

        $appPath = Join-Path $componentsDir "App.razor"
        Set-Content -Path $appPath -Value $appRazorContent -Encoding UTF8
        Write-TransformLog -File $appPath -Transform 'Scaffold' -Detail 'Generated Components/App.razor'

        $routesPath = Join-Path $componentsDir "Routes.razor"
        Set-Content -Path $routesPath -Value $routesRazorContent -Encoding UTF8
        Write-TransformLog -File $routesPath -Transform 'Scaffold' -Detail 'Generated Components/Routes.razor'
    }
    else {
        Write-Host "[WhatIf] Would create: Components/App.razor"
        Write-Host "[WhatIf] Would create: Components/Routes.razor"
    }
}

#endregion

#region --- Directive Transforms ---

function ConvertFrom-PageDirective {
    param([string]$Content, [string]$FileName, [string]$RelPath)

    # <%@ Page ... %> → @page "/route"
    $route = '/' + [System.IO.Path]::GetFileNameWithoutExtension($FileName)
    if ($route -eq '/Default' -or $route -eq '/default' -or $route -eq '/Index' -or $route -eq '/index') {
        $route = '/'
    }

    if ($Content -match '<%@\s*Page[^%]*%>') {
        $Content = $Content -replace '<%@\s*Page[^%]*%>\s*\r?\n?', ''
        $Content = "@page `"$route`"`n" + $Content
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "<%@ Page %> → @page `"$route`""
    }
    return $Content
}

function ConvertFrom-MasterDirective {
    param([string]$Content, [string]$RelPath)

    if ($Content -match '<%@\s*Master[^%]*%>') {
        $Content = $Content -replace '<%@\s*Master[^%]*%>\s*\r?\n?', ''
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail 'Removed <%@ Master %>'
    }
    return $Content
}

function ConvertFrom-ControlDirective {
    param([string]$Content, [string]$RelPath)

    if ($Content -match '<%@\s*Control[^%]*%>') {
        $Content = $Content -replace '<%@\s*Control[^%]*%>\s*\r?\n?', ''
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail 'Removed <%@ Control %>'
    }
    return $Content
}

function ConvertFrom-RegisterDirective {
    param([string]$Content, [string]$RelPath)

    $regex = [regex]'<%@\s*Register[^%]*%>\s*\r?\n?'
    $matches_ = $regex.Matches($Content)
    foreach ($m in $matches_) {
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "Removed <%@ Register %> (review component references)"
        Write-ManualItem -File $RelPath -Category 'Register' -Detail "Removed Register directive — verify component tag prefixes: $($m.Value.Trim())"
    }
    $Content = $regex.Replace($Content, '')
    return $Content
}

function ConvertFrom-ImportDirective {
    param([string]$Content, [string]$RelPath)

    $regex = [regex]'<%@\s*Import\s+Namespace="([^"]+)"\s*%>\s*\r?\n?'
    $matches_ = $regex.Matches($Content)
    foreach ($m in $matches_) {
        $ns = $m.Groups[1].Value
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "<%@ Import Namespace=`"$ns`" %> → @using $ns"
    }
    $Content = $regex.Replace($Content, { param($m) "@using $($m.Groups[1].Value)`n" })
    return $Content
}

#endregion

#region --- Content & Form Transforms ---

function ConvertFrom-ContentWrappers {
    param([string]$Content, [string]$RelPath)

    # HeadContent placeholder → <HeadContent> / </HeadContent>
    $headOpenRegex = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"HeadContent"[^>]*>'
    $headOpenRegex2 = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"head"[^>]*>'
    if ($Content -match $headOpenRegex -or $Content -match $headOpenRegex2) {
        $Content = $headOpenRegex.Replace($Content, '<HeadContent>')
        $Content = $headOpenRegex2.Replace($Content, '<HeadContent>')
        Write-TransformLog -File $RelPath -Transform 'Content' -Detail 'HeadContent placeholder → <HeadContent>'
    }

    # MainContent / other ContentPlaceHolderIDs → strip wrapper entirely
    $mainRegex = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"[^"]*"[^>]*>\s*\r?\n?'
    $matches_ = $mainRegex.Matches($Content)
    foreach ($m in $matches_) {
        Write-TransformLog -File $RelPath -Transform 'Content' -Detail "Removed asp:Content open tag"
    }
    $Content = $mainRegex.Replace($Content, '')

    # Closing </asp:Content> tags
    $closeRegex = [regex]'</asp:Content>\s*\r?\n?'
    $closeCount = $closeRegex.Matches($Content).Count
    if ($closeCount -gt 0) {
        # Keep matching number of </HeadContent> if we converted HeadContent above
        $headContentCount = ([regex]'<HeadContent>').Matches($Content).Count
        if ($headContentCount -gt 0) {
            # Replace first N closing tags with </HeadContent>, remove the rest
            $replaced = 0
            $Content = $closeRegex.Replace($Content, {
                param($m)
                $script:replaced_count++
                if ($script:replaced_count -le $headContentCount) {
                    return "</HeadContent>`n"
                }
                return ''
            })
            # Reset and redo with proper scoping
            $script:replaced_count = 0
            $tempContent = $Content
            $Content = ''
            $lastIndex = 0
            $closeMatches = $closeRegex.Matches($tempContent)
            $headCount = 0
            foreach ($cm in $closeMatches) {
                $Content += $tempContent.Substring($lastIndex, $cm.Index - $lastIndex)
                $headCount++
                if ($headCount -le $headContentCount) {
                    $Content += "</HeadContent>`n"
                }
                $lastIndex = $cm.Index + $cm.Length
            }
            $Content += $tempContent.Substring($lastIndex)
        }
        else {
            $Content = $closeRegex.Replace($Content, '')
        }
        Write-TransformLog -File $RelPath -Transform 'Content' -Detail "Removed $closeCount </asp:Content> closing tag(s)"
    }

    return $Content
}

function ConvertFrom-FormWrapper {
    param([string]$Content, [string]$RelPath)

    # Remove <form ... runat="server" ...> and its closing </form> — but only the server form
    $formOpenRegex = [regex]'<form\s+[^>]*runat\s*=\s*"server"[^>]*>\s*\r?\n?'
    if ($Content -match $formOpenRegex) {
        $Content = $formOpenRegex.Replace($Content, '', 1)
        # Remove the corresponding closing </form> (the last one, or the first one after removal)
        # Simple approach: remove one </form> tag
        $formCloseRegex = [regex]'</form>\s*\r?\n?'
        $Content = $formCloseRegex.Replace($Content, '', 1)
        Write-TransformLog -File $RelPath -Transform 'Form' -Detail 'Removed <form runat="server"> and </form>'
    }
    return $Content
}

#endregion

#region --- Master Page Transforms ---

function ConvertFrom-MasterPage {
    param([string]$Content, [string]$RelPath)

    # 1. Remove <asp:ScriptManager> block (can be multi-line with nested <Scripts>)
    $smRegex = [regex]'(?s)<asp:ScriptManager[^>]*>.*?</asp:ScriptManager>\s*\r?\n?'
    if ($Content -match '<asp:ScriptManager') {
        $Content = $smRegex.Replace($Content, '')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'Removed <asp:ScriptManager> block'
    }
    # Also handle self-closing ScriptManager
    $smSelfRegex = [regex]'<asp:ScriptManager[^>]*/>\s*\r?\n?'
    $Content = $smSelfRegex.Replace($Content, '')

    # 2. Extract head metadata (<meta>, <link>, <title>) before stripping <head> section
    $headContentBlock = ''
    $headSectionRegex = [regex]'(?s)<head[^>]*>(.*?)</head>'
    $headMatch = $headSectionRegex.Match($Content)
    if ($headMatch.Success) {
        $headInner = $headMatch.Groups[1].Value
        $extractedTags = [System.Collections.Generic.List[string]]::new()

        foreach ($m in ([regex]'<meta\s[^>]*>').Matches($headInner)) {
            $extractedTags.Add("    " + $m.Value.Trim())
        }
        foreach ($m in ([regex]'(?s)<title>.*?</title>').Matches($headInner)) {
            $extractedTags.Add("    " + $m.Value.Trim())
        }
        foreach ($m in ([regex]'<link\s[^>]*>').Matches($headInner)) {
            $extractedTags.Add("    " + $m.Value.Trim())
        }

        if ($extractedTags.Count -gt 0) {
            $headContentBlock = "<HeadContent>`n" + ($extractedTags -join "`n") + "`n</HeadContent>"
            Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "Extracted $($extractedTags.Count) head element(s) into <HeadContent>"
        }

        # Remove the entire <head>...</head> section
        $Content = $headSectionRegex.Replace($Content, '')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'Removed <head> section'
    }

    # 3. Strip document wrapper tags
    $Content = $Content -replace '<!DOCTYPE[^>]*>\s*\r?\n?', ''
    $Content = $Content -replace '<html[^>]*>\s*\r?\n?', ''
    $Content = $Content -replace '</html>\s*\r?\n?', ''
    $Content = $Content -replace '<body[^>]*>\s*\r?\n?', ''
    $Content = $Content -replace '</body>\s*\r?\n?', ''
    Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'Stripped document wrapper (DOCTYPE, html, body)'

    # 4. Replace <asp:ContentPlaceHolder ID="MainContent"> → @Body
    $mainCphRegex = [regex]'(?si)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"MainContent"[^>]*>.*?</asp:ContentPlaceHolder>'
    if ($mainCphRegex.IsMatch($Content)) {
        $Content = $mainCphRegex.Replace($Content, '@Body')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'ContentPlaceHolder MainContent → @Body'
    }
    # Self-closing MainContent
    $mainCphSelfRegex = [regex]'(?i)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"MainContent"[^>]*/>'
    if ($mainCphSelfRegex.IsMatch($Content)) {
        $Content = $mainCphSelfRegex.Replace($Content, '@Body')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'ContentPlaceHolder MainContent → @Body (self-closing)'
    }

    # Other ContentPlaceHolders → TODO comment
    $otherCphRegex = [regex]'(?si)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"([^"]+)"[^>]*>.*?</asp:ContentPlaceHolder>'
    foreach ($m in $otherCphRegex.Matches($Content)) {
        Write-ManualItem -File $RelPath -Category 'ContentPlaceHolder' -Detail "Non-MainContent ContentPlaceHolder ID='$($m.Groups[1].Value)' needs manual conversion"
    }
    $Content = $otherCphRegex.Replace($Content, { param($m) "@* TODO: ContentPlaceHolder '$($m.Groups[1].Value)' — convert to a section or nested layout *@" })
    # Self-closing other ContentPlaceHolders
    $otherCphSelfRegex = [regex]'(?i)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"([^"]+)"[^>]*/>'
    foreach ($m in $otherCphSelfRegex.Matches($Content)) {
        Write-ManualItem -File $RelPath -Category 'ContentPlaceHolder' -Detail "Non-MainContent ContentPlaceHolder ID='$($m.Groups[1].Value)' needs manual conversion (self-closing)"
    }
    $Content = $otherCphSelfRegex.Replace($Content, { param($m) "@* TODO: ContentPlaceHolder '$($m.Groups[1].Value)' — convert to a section or nested layout *@" })

    # 5. Flag items needing Layer 2 attention
    if ($Content -match '<RoleGroups>') {
        Write-ManualItem -File $RelPath -Category 'LoginView-RoleGroups' -Detail 'LoginView <RoleGroups> requires manual conversion to @attribute [Authorize(Roles="...")]'
    }
    if ($Content -match 'SelectMethod\s*=') {
        Write-ManualItem -File $RelPath -Category 'SelectMethod' -Detail 'SelectMethod detected — will be auto-converted to TODO annotation by ConvertFrom-SelectMethod'
    }

    # 6. Inject @inherits LayoutComponentBase and HeadContent at the top
    $header = "@inherits LayoutComponentBase`n"
    if ($headContentBlock) {
        $header += "`n" + $headContentBlock + "`n"
    }
    $Content = $header + "`n" + $Content

    return $Content
}

#endregion

#region --- Expression Transforms ---

function ConvertFrom-Expressions {
    param([string]$Content, [string]$RelPath)

    # Comments: <%-- ... --%> → @* ... *@
    $commentRegex = [regex]'(?s)<%--(.+?)--%>'
    $commentMatches = $commentRegex.Matches($Content)
    if ($commentMatches.Count -gt 0) {
        $Content = $commentRegex.Replace($Content, '@*$1*@')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($commentMatches.Count) comment(s) to Razor syntax"
    }

    # Data binding with Eval and format string: <%#: Eval("prop", "{0:fmt}") %> → @context.prop.ToString("fmt")
    $evalFmtRegex = [regex]'<%#:\s*Eval\("(\w+)",\s*"\{0:([^}]+)\}"\)\s*%>'
    $evalFmtMatches = $evalFmtRegex.Matches($Content)
    if ($evalFmtMatches.Count -gt 0) {
        $Content = $evalFmtRegex.Replace($Content, '@context.$1.ToString("$2")')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($evalFmtMatches.Count) Eval() with format string to @context.ToString()"
    }

    # String.Format with Item.Property: <%#: String.Format("{0:fmt}", Item.Prop) %> → @($"{context.Prop:fmt}")
    $strFmtRegex = [regex]'<%#:\s*String\.Format\("\{0:([^}]+)\}",\s*Item\.(\w+)\)\s*%>'
    $strFmtMatches = $strFmtRegex.Matches($Content)
    if ($strFmtMatches.Count -gt 0) {
        $Content = $strFmtRegex.Replace($Content, '@($$"{context.$2:$1}")')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($strFmtMatches.Count) String.Format(Item.) to interpolated string"
    }

    # Data binding with Eval: <%#:\s*Eval("prop")\s*%> → @context.prop
    $evalRegex = [regex]'<%#:\s*Eval\("(\w+)"\)\s*%>'
    $evalMatches = $evalRegex.Matches($Content)
    if ($evalMatches.Count -gt 0) {
        $Content = $evalRegex.Replace($Content, '@context.$1')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($evalMatches.Count) Eval() binding(s) to @context"
    }

    # Data binding with Item: <%#:\s*Item.Prop\s*%> → @context.Prop
    $itemRegex = [regex]'<%#:\s*Item\.(\w+)\s*%>'
    $itemMatches = $itemRegex.Matches($Content)
    if ($itemMatches.Count -gt 0) {
        $Content = $itemRegex.Replace($Content, '@context.$1')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($itemMatches.Count) Item binding(s) to @context"
    }

    # Encoded expressions: <%: expr %> → @(expr)
    $encodedRegex = [regex]'<%:\s*(.+?)\s*%>'
    $encodedMatches = $encodedRegex.Matches($Content)
    if ($encodedMatches.Count -gt 0) {
        $Content = $encodedRegex.Replace($Content, '@($1)')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($encodedMatches.Count) encoded expression(s)"
    }

    # Unencoded expressions: <%= expr %> → @(expr)
    $unencodedRegex = [regex]'<%=\s*(.+?)\s*%>'
    $unencodedMatches = $unencodedRegex.Matches($Content)
    if ($unencodedMatches.Count -gt 0) {
        $Content = $unencodedRegex.Replace($Content, '@($1)')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($unencodedMatches.Count) unencoded expression(s)"
    }

    # Flag any remaining <% ... %> blocks as manual
    $remainingRegex = [regex]'<%[^@].*?%>'
    $remainingMatches = $remainingRegex.Matches($Content)
    foreach ($m in $remainingMatches) {
        Write-ManualItem -File $RelPath -Category 'CodeBlock' -Detail "Unconverted code block: $($m.Value.Substring(0, [Math]::Min(80, $m.Value.Length)))"
    }

    return $Content
}

#endregion

#region --- LoginView Conversion ---

function ConvertFrom-LoginView {
    param([string]$Content, [string]$RelPath)

    # Flag <RoleGroups> as manual — too complex for regex
    if ($Content -match '<RoleGroups>') {
        Write-ManualItem -File $RelPath -Category 'LoginView-RoleGroups' -Detail 'LoginView <RoleGroups> requires manual conversion to @attribute [Authorize(Roles="...")]'
    }

    # <asp:LoginView ...> → <AuthorizeView> (strip all attributes)
    $openRegex = [regex]'(?i)<asp:LoginView\b[^>]*>'
    $openMatches = $openRegex.Matches($Content)
    if ($openMatches.Count -gt 0) {
        $Content = $openRegex.Replace($Content, '<AuthorizeView>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($openMatches.Count) <asp:LoginView> to <AuthorizeView>"
    }

    # </asp:LoginView> → </AuthorizeView>
    $closeRegex = [regex]'(?i)</asp:LoginView\s*>'
    $closeMatches = $closeRegex.Matches($Content)
    if ($closeMatches.Count -gt 0) {
        $Content = $closeRegex.Replace($Content, '</AuthorizeView>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($closeMatches.Count) </asp:LoginView> to </AuthorizeView>"
    }

    # <AnonymousTemplate> → <NotAuthorized>
    $anonOpenRegex = [regex]'(?i)<AnonymousTemplate\s*>'
    $anonOpenMatches = $anonOpenRegex.Matches($Content)
    if ($anonOpenMatches.Count -gt 0) {
        $Content = $anonOpenRegex.Replace($Content, '<NotAuthorized>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($anonOpenMatches.Count) <AnonymousTemplate> to <NotAuthorized>"
    }

    # </AnonymousTemplate> → </NotAuthorized>
    $anonCloseRegex = [regex]'(?i)</AnonymousTemplate\s*>'
    $anonCloseMatches = $anonCloseRegex.Matches($Content)
    if ($anonCloseMatches.Count -gt 0) {
        $Content = $anonCloseRegex.Replace($Content, '</NotAuthorized>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($anonCloseMatches.Count) </AnonymousTemplate> to </NotAuthorized>"
    }

    # <LoggedInTemplate> → <Authorized>
    $loggedOpenRegex = [regex]'(?i)<LoggedInTemplate\s*>'
    $loggedOpenMatches = $loggedOpenRegex.Matches($Content)
    if ($loggedOpenMatches.Count -gt 0) {
        $Content = $loggedOpenRegex.Replace($Content, '<Authorized>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($loggedOpenMatches.Count) <LoggedInTemplate> to <Authorized>"
    }

    # </LoggedInTemplate> → </Authorized>
    $loggedCloseRegex = [regex]'(?i)</LoggedInTemplate\s*>'
    $loggedCloseMatches = $loggedCloseRegex.Matches($Content)
    if ($loggedCloseMatches.Count -gt 0) {
        $Content = $loggedCloseRegex.Replace($Content, '</Authorized>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($loggedCloseMatches.Count) </LoggedInTemplate> to </Authorized>"
    }

    return $Content
}

#endregion

#region --- GetRouteUrl Conversion ---

function ConvertFrom-GetRouteUrl {
    param([string]$Content, [string]$RelPath)

    $transformed = $false

    # Page.GetRouteUrl( → GetRouteUrlHelper.GetRouteUrl(
    $pageRouteRegex = [regex]'Page\.GetRouteUrl\s*\('
    $pageRouteMatches = $pageRouteRegex.Matches($Content)
    if ($pageRouteMatches.Count -gt 0) {
        $Content = $pageRouteRegex.Replace($Content, 'GetRouteUrlHelper.GetRouteUrl(')
        Write-TransformLog -File $RelPath -Transform 'GetRouteUrl' -Detail "Converted $($pageRouteMatches.Count) Page.GetRouteUrl() to GetRouteUrlHelper.GetRouteUrl()"
        $transformed = $true
    }

    # Standalone GetRouteUrl( (not already prefixed with Helper.) → GetRouteUrlHelper.GetRouteUrl(
    $standaloneRegex = [regex]'(?<![\w.])GetRouteUrl\s*\('
    $standaloneMatches = $standaloneRegex.Matches($Content)
    if ($standaloneMatches.Count -gt 0) {
        $Content = $standaloneRegex.Replace($Content, 'GetRouteUrlHelper.GetRouteUrl(')
        Write-TransformLog -File $RelPath -Transform 'GetRouteUrl' -Detail "Converted $($standaloneMatches.Count) GetRouteUrl() to GetRouteUrlHelper.GetRouteUrl()"
        $transformed = $true
    }

    # Inside route value arguments, convert Eval("Prop") to context.Prop
    $evalInRouteRegex = [regex]'Eval\("(\w+)"\)'
    $evalInRouteMatches = $evalInRouteRegex.Matches($Content)
    if ($evalInRouteMatches.Count -gt 0) {
        $Content = $evalInRouteRegex.Replace($Content, 'context.$1')
        Write-TransformLog -File $RelPath -Transform 'GetRouteUrl' -Detail "Converted $($evalInRouteMatches.Count) Eval() in route values to context.Property"
    }

    # Flag RouteValueDictionary usage as manual
    if ($Content -match 'RouteValueDictionary') {
        Write-ManualItem -File $RelPath -Category 'GetRouteUrl' -Detail 'RouteValueDictionary usage detected — works but consider simplifying to anonymous object'
    }

    # Note the required @inject directive
    if ($transformed) {
        Write-ManualItem -File $RelPath -Category 'GetRouteUrl' -Detail 'Add @inject GetRouteUrlHelper GetRouteUrlHelper at the top of the file'
    }

    return $Content
}

#endregion

#region --- SelectMethod Conversion ---

function ConvertFrom-SelectMethod {
    param([string]$Content, [string]$RelPath)

    # Match SelectMethod="MethodName" in any tag, capture the method name and insert a TODO after the tag
    $selectMethodRegex = [regex]'(?si)(<[^>]*?)\s+SelectMethod\s*=\s*"([^"]+)"([^>]*>)'
    $selectMethodMatches = $selectMethodRegex.Matches($Content)
    if ($selectMethodMatches.Count -gt 0) {
        $Content = $selectMethodRegex.Replace($Content, {
            param($m)
            $tagBeforeAttr = $m.Groups[1].Value
            $methodName = $m.Groups[2].Value
            $tagAfterAttr = $m.Groups[3].Value
            $serviceName = 'I' + $methodName.TrimStart('Get') + 'Service'
            $varName = ($methodName.TrimStart('Get')).Substring(0,1).ToLower() + ($methodName.TrimStart('Get')).Substring(1) + 'Service'
            "${tagBeforeAttr}${tagAfterAttr}`n@* TODO: SelectMethod=""${methodName}"" removed — inject a service that provides this data:`n   @inject ${serviceName} ${varName}`n   Then in @code { ... OnInitializedAsync() { items = await ${varName}.${methodName}(); } }`n*@"
        })
        Write-TransformLog -File $RelPath -Transform 'SelectMethod' -Detail "Converted $($selectMethodMatches.Count) SelectMethod attribute(s) to TODO annotations"
        foreach ($m in $selectMethodMatches) {
            Write-ManualItem -File $RelPath -Category 'SelectMethod' -Detail "SelectMethod='$($m.Groups[2].Value)' removed — needs service injection and OnInitializedAsync data loading"
        }
    }

    return $Content
}

#endregion

#region --- Tag & Attribute Transforms ---

function ConvertFrom-AspPrefix {
    param([string]$Content, [string]$RelPath)

    # Opening tags: <asp:Button → <Button
    $openRegex = [regex]'<asp:(\w+)'
    $openMatches = $openRegex.Matches($Content)
    if ($openMatches.Count -gt 0) {
        $Content = $openRegex.Replace($Content, '<$1')
        Write-TransformLog -File $RelPath -Transform 'TagPrefix' -Detail "Removed asp: prefix from $($openMatches.Count) opening tag(s)"
    }

    # Closing tags: </asp:Button> → </Button>
    $closeRegex = [regex]'</asp:(\w+)>'
    $closeMatches = $closeRegex.Matches($Content)
    if ($closeMatches.Count -gt 0) {
        $Content = $closeRegex.Replace($Content, '</$1>')
        Write-TransformLog -File $RelPath -Transform 'TagPrefix' -Detail "Removed asp: prefix from $($closeMatches.Count) closing tag(s)"
    }

    return $Content
}

function Remove-WebFormsAttributes {
    param([string]$Content, [string]$RelPath)

    foreach ($pattern in $StripAttributes) {
        $attrRegex = [regex]"\s*$pattern"
        $attrMatches = $attrRegex.Matches($Content)
        if ($attrMatches.Count -gt 0) {
            $Content = $attrRegex.Replace($Content, '')
            # Extract a friendly name from the pattern for logging
            $friendlyName = $pattern -replace '\\s\*=\\s\*.*', '' -replace '\\s\*', ' ' -replace '\\', ''
            Write-TransformLog -File $RelPath -Transform 'Attribute' -Detail "Removed $($attrMatches.Count) '$friendlyName' attribute(s)"
        }
    }

    # ItemType="Namespace.Class" → TItem="Class"
    $itemTypeRegex = [regex]'ItemType="(?:[^"]*\.)?([^"]+)"'
    $itemTypeMatches = $itemTypeRegex.Matches($Content)
    if ($itemTypeMatches.Count -gt 0) {
        $Content = $itemTypeRegex.Replace($Content, 'TItem="$1"')
        Write-TransformLog -File $RelPath -Transform 'Attribute' -Detail "Converted $($itemTypeMatches.Count) ItemType to TItem"
    }

    return $Content
}

function ConvertFrom-UrlReferences {
    param([string]$Content, [string]$RelPath)

    $urlPatterns = @(
        @{ Pattern = 'href="~/';       Replacement = 'href="/';       Name = 'href' }
        @{ Pattern = 'NavigateUrl="~/'; Replacement = 'NavigateUrl="/'; Name = 'NavigateUrl' }
        @{ Pattern = 'ImageUrl="~/';    Replacement = 'ImageUrl="/';    Name = 'ImageUrl' }
    )

    foreach ($up in $urlPatterns) {
        $count = ([regex]::Matches($Content, [regex]::Escape($up.Pattern))).Count
        if ($count -gt 0) {
            $Content = $Content.Replace($up.Pattern, $up.Replacement)
            Write-TransformLog -File $RelPath -Transform 'URL' -Detail "Converted $count $($up.Name) ~/ reference(s) to /"
        }
    }

    return $Content
}

#endregion

#region --- Code-Behind Handling ---

function Copy-CodeBehind {
    param(
        [string]$SourceFile,
        [string]$OutputFile,
        [string]$RelPath
    )

    if ($PSCmdlet.ShouldProcess($OutputFile, "Copy code-behind with TODO annotations")) {
        $content = Get-Content -Path $SourceFile -Raw -Encoding UTF8

        $todoHeader = @"
// =============================================================================
// TODO: This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   - Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   - Page_PreRender → OnAfterRenderAsync
//   - IsPostBack checks → remove or convert to state logic
//   - ViewState usage → component [Parameter] or private fields
//   - Session/Cache access → inject IHttpContextAccessor or use DI
//   - Response.Redirect → NavigationManager.NavigateTo
//   - Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   - Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   - UpdatePanel / ScriptManager references → remove (Blazor handles updates)
//   - User controls → Blazor component references
// =============================================================================

"@

        $annotatedContent = $todoHeader + $content

        $outputDir = Split-Path $OutputFile -Parent
        if (-not (Test-Path $outputDir)) {
            New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
        }

        Set-Content -Path $OutputFile -Value $annotatedContent -Encoding UTF8
        Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Copied with TODO annotations → $OutputFile"
    }
    else {
        Write-Host "[WhatIf] Would copy code-behind: $RelPath → $OutputFile"
    }
}

#endregion

#region --- Main File Transform Pipeline ---

function Convert-WebFormsFile {
    param(
        [string]$SourceFile,
        [string]$OutputRoot,
        [string]$SourceRoot
    )

    $relativePath = $SourceFile.Substring($SourceRoot.Length).TrimStart('\', '/')
    $extension = [System.IO.Path]::GetExtension($SourceFile).ToLower()
    $fileName = [System.IO.Path]::GetFileName($SourceFile)

    if ($VerbosePreference -eq 'Continue') {
        Write-Verbose "Processing: $relativePath"
    }

    # Determine output path with .razor extension
    $razorRelPath = $relativePath
    switch ($extension) {
        '.aspx'   { $razorRelPath = $razorRelPath -replace '\.aspx$', '.razor' }
        '.ascx'   { $razorRelPath = $razorRelPath -replace '\.ascx$', '.razor' }
        '.master' {
            $baseName = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
            if ($baseName -eq 'Site') {
                $razorRelPath = 'Components\Layout\MainLayout.razor'
            } else {
                $razorRelPath = "Components\Layout\${baseName}Layout.razor"
            }
        }
    }

    $outputFile = Join-Path $OutputRoot $razorRelPath
    $outputDir = Split-Path $outputFile -Parent

    # Read source content
    $content = Get-Content -Path $SourceFile -Raw -Encoding UTF8
    if ([string]::IsNullOrEmpty($content)) {
        Write-Warning "Skipping empty file: $relativePath"
        return
    }

    $script:FilesProcessed++

    # Apply transform pipeline in order
    switch ($extension) {
        '.aspx' {
            $content = ConvertFrom-PageDirective -Content $content -FileName $fileName -RelPath $relativePath
        }
        '.master' {
            $content = ConvertFrom-MasterDirective -Content $content -RelPath $relativePath
            $content = ConvertFrom-MasterPage -Content $content -RelPath $relativePath
        }
        '.ascx' {
            $content = ConvertFrom-ControlDirective -Content $content -RelPath $relativePath
        }
    }

    $content = ConvertFrom-ImportDirective -Content $content -RelPath $relativePath
    $content = ConvertFrom-RegisterDirective -Content $content -RelPath $relativePath
    $content = ConvertFrom-ContentWrappers -Content $content -RelPath $relativePath
    $content = ConvertFrom-FormWrapper -Content $content -RelPath $relativePath
    $content = ConvertFrom-GetRouteUrl -Content $content -RelPath $relativePath
    $content = ConvertFrom-Expressions -Content $content -RelPath $relativePath
    $content = ConvertFrom-LoginView -Content $content -RelPath $relativePath
    $content = ConvertFrom-SelectMethod -Content $content -RelPath $relativePath
    $content = ConvertFrom-AspPrefix -Content $content -RelPath $relativePath
    $content = Remove-WebFormsAttributes -Content $content -RelPath $relativePath
    $content = ConvertFrom-UrlReferences -Content $content -RelPath $relativePath

    # Clean up leftover blank lines from removed directives (collapse 3+ consecutive blank lines to 2)
    $content = $content -replace '(\r?\n){3,}', "`n`n"
    $content = $content.TrimStart("`r", "`n")

    Write-TransformLog -File $relativePath -Transform 'Rename' -Detail "$extension → .razor"

    # Write output
    if ($PSCmdlet.ShouldProcess($outputFile, "Write transformed Razor file")) {
        if (-not (Test-Path $outputDir)) {
            New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
        }
        Set-Content -Path $outputFile -Value $content -Encoding UTF8
    }
    else {
        Write-Host "[WhatIf] Would write: $razorRelPath ($($script:TransformLog.Count) transforms)"
    }

    # Handle code-behind files (.aspx.cs, .aspx.vb, etc.)
    $relevantCbExtensions = $CodeBehindExtensions | Where-Object { $_.StartsWith($extension + '.') }
    foreach ($cbExt in $relevantCbExtensions) {
        $cbSuffix = $cbExt.Substring($extension.Length)  # e.g., ".cs" or ".vb"
        $cbSource = $SourceFile + $cbSuffix

        if (Test-Path $cbSource) {
            $cbRelPath = $cbSource.Substring($SourceRoot.Length).TrimStart('\', '/')
            $cbOutputRel = $cbRelPath
            switch ($extension) {
                '.aspx'   { $cbOutputRel = $cbOutputRel -replace '\.aspx\.', '.razor.' }
                '.ascx'   { $cbOutputRel = $cbOutputRel -replace '\.ascx\.', '.razor.' }
                '.master' { $cbOutputRel = $razorRelPath + $cbSuffix }
            }
            $cbOutputFile = Join-Path $OutputRoot $cbOutputRel
            Copy-CodeBehind -SourceFile $cbSource -OutputFile $cbOutputFile -RelPath $cbRelPath
        }
    }
}

#endregion

#region --- Entry Point ---

# Resolve paths
$Path = (Resolve-Path $Path -ErrorAction Stop).Path
if (-not (Test-Path $Path -PathType Container)) {
    Write-Error "Source path is not a directory: $Path"
    return
}

$projectName = Split-Path $Path -Leaf
if (-not $projectName) {
    $projectName = 'BlazorApp'
}
# Sanitize project name for C# namespace
$projectName = $projectName -replace '[^a-zA-Z0-9_]', ''
if ($projectName -match '^\d') {
    $projectName = '_' + $projectName
}
if ([string]::IsNullOrEmpty($projectName)) {
    $projectName = 'BlazorApp'
}

Write-Host ''
Write-Host '============================================================' -ForegroundColor Cyan
Write-Host '  BWFC Migration Tool — Layer 1: Mechanical Transforms' -ForegroundColor Cyan
Write-Host '============================================================' -ForegroundColor Cyan
Write-Host "  Source:  $Path"
Write-Host "  Output:  $Output"
Write-Host "  Project: $projectName"
if ($WhatIfPreference) {
    Write-Host '  Mode:    WhatIf (dry run)' -ForegroundColor Yellow
}
Write-Host ''

# Create output directory
if (-not $WhatIfPreference) {
    if (-not (Test-Path $Output)) {
        New-Item -ItemType Directory -Path $Output -Force | Out-Null
        Write-Host "Created output directory: $Output"
    }
    $Output = (Resolve-Path $Output).Path
}

# Project scaffolding
if (-not $SkipProjectScaffold) {
    Write-Host 'Generating project scaffold...' -ForegroundColor Green
    if (-not $WhatIfPreference) {
        New-ProjectScaffold -OutputRoot $Output -ProjectName $projectName
        New-AppRazorScaffold -OutputRoot $Output -ProjectName $projectName
    }
    else {
        Write-Host '[WhatIf] Would generate .csproj, _Imports.razor, Program.cs, App.razor, Routes.razor'
    }
    Write-Host ''
}

# Discover and transform Web Forms files
Write-Host 'Discovering Web Forms files...' -ForegroundColor Green
$sourceFiles = Get-ChildItem -Path $Path -Recurse -File | Where-Object {
    $ext = $_.Extension.ToLower()
    $ext -in $WebFormsExtensions
}

$fileCount = ($sourceFiles | Measure-Object).Count
Write-Host "Found $fileCount Web Forms file(s) to transform."
Write-Host ''

if ($fileCount -gt 0) {
    Write-Host 'Applying transforms...' -ForegroundColor Green
    foreach ($file in $sourceFiles) {
        Convert-WebFormsFile -SourceFile $file.FullName -OutputRoot $Output -SourceRoot $Path
    }
    Write-Host ''
}

# Copy static files (css, js, images)
$staticFiles = Get-ChildItem -Path $Path -Recurse -File | Where-Object {
    $ext = $_.Extension.ToLower()
    $ext -in $StaticExtensions
}
$staticCount = ($staticFiles | Measure-Object).Count
if ($staticCount -gt 0) {
    Write-Host "Copying $staticCount static file(s)..." -ForegroundColor Green
    foreach ($sf in $staticFiles) {
        $relPath = $sf.FullName.Substring($Path.Length).TrimStart('\', '/')
        $destPath = Join-Path $Output $relPath
        $destDir = Split-Path $destPath -Parent

        if ($PSCmdlet.ShouldProcess($destPath, "Copy static file")) {
            if (-not (Test-Path $destDir)) {
                New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            }
            Copy-Item -Path $sf.FullName -Destination $destPath -Force
        }
        else {
            Write-Host "[WhatIf] Would copy: $relPath"
        }
    }
    Write-Host ''
}

#endregion

#region --- Summary ---

Write-Host '============================================================' -ForegroundColor Cyan
Write-Host '  Migration Summary' -ForegroundColor Cyan
Write-Host '============================================================' -ForegroundColor Cyan
Write-Host "  Files processed:       $($script:FilesProcessed)"
Write-Host "  Transforms applied:    $($script:TransformsApplied)"
Write-Host "  Static files copied:   $staticCount"
Write-Host "  Items needing review:  $($script:ManualItems.Count)"
Write-Host ''

if ($script:ManualItems.Count -gt 0) {
    Write-Host '--- Items Needing Manual Attention ---' -ForegroundColor Yellow
    $grouped = $script:ManualItems | Group-Object -Property Category
    foreach ($group in $grouped) {
        Write-Host "  [$($group.Name)] ($($group.Count) item(s)):" -ForegroundColor Yellow
        foreach ($item in $group.Group) {
            Write-Host "    • $($item.File): $($item.Detail)"
        }
    }
    Write-Host ''
}

if ($VerbosePreference -eq 'Continue' -and $script:TransformLog.Count -gt 0) {
    Write-Host '--- Detailed Transform Log ---' -ForegroundColor DarkGray
    $groupedByFile = $script:TransformLog | Group-Object -Property File
    foreach ($fileGroup in $groupedByFile) {
        Write-Host "  $($fileGroup.Name):" -ForegroundColor DarkGray
        foreach ($entry in $fileGroup.Group) {
            Write-Host "    [$($entry.Transform)] $($entry.Detail)" -ForegroundColor DarkGray
        }
    }
    Write-Host ''
}

if (-not $WhatIfPreference) {
    Write-Host 'Migration complete. Next steps:' -ForegroundColor Green
    Write-Host '  1. Review items flagged above for manual attention'
    Write-Host '  2. Use the BWFC Copilot skill for code-behind transforms (Layer 2)'
    Write-Host '  3. Build and test: dotnet build && dotnet run'
}
else {
    Write-Host 'Dry run complete. Run without -WhatIf to apply transforms.' -ForegroundColor Yellow
}

Write-Host ''

#endregion
