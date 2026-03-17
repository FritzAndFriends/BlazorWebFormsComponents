# ContosoUniversity Web Forms — Local Setup Guide

> Reference setup for the ASP.NET Web Forms sample application used by the BWFC migration showcase.

## Prerequisites

| Tool | Required | Location on Build Machine |
|------|----------|--------------------------|
| **IIS Express** | ✅ | `C:\Program Files\IIS Express\iisexpress.exe` |
| **SQL Server LocalDB** | ✅ | Instance `MSSQLLocalDB` (SQL Server 2022 / v17.0) |
| **MSBuild 18.x** (with WebApplication targets) | ✅ | `C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe` |
| **NuGet CLI** | ✅ | `D:\BlazorWebFormsComponents\nuget.exe` (repo-root) |
| **sqlcmd** | ✅ | `C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE` |

> **Note:** MSBuild from VS 2017 BuildTools (v15.0) lacks `Microsoft.WebApplication.targets` and cannot build this project. Use MSBuild from Visual Studio 18 Insiders (or any VS edition that includes web workloads).

## Step-by-Step Reproduction

### 1. Start LocalDB and Attach the Database

```powershell
sqllocaldb start MSSQLLocalDB

sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "
  CREATE DATABASE ContosoUniversity
  ON (FILENAME = 'D:\BlazorWebFormsComponents\samples\ContosoUniversity\ContosoUniversity\App_Data\ContosoUniversity.mdf')
  FOR ATTACH_REBUILD_LOG;
"
```

The `.mdf` ships from an older SQL Server version (internal version 782). LocalDB auto-upgrades it to version 998 and creates a new log file.

### 2. Restore NuGet Packages

```powershell
cd samples\ContosoUniversity
..\..\nuget.exe restore ContosoUniversity.sln
```

### 3. Build

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" `
    ContosoUniversity\ContosoUniversity.csproj `
    /p:Configuration=Debug /v:minimal
```

### 4. Launch IIS Express

```powershell
& "C:\Program Files\IIS Express\iisexpress.exe" `
    /path:"D:\BlazorWebFormsComponents\samples\ContosoUniversity\ContosoUniversity" `
    /port:44380
```

Base URL: **http://localhost:44380/**

Press `Q` in the IIS Express console to stop the server.

### 5. Verify Pages

Navigate to each page to confirm the app is working:

| Page | URL | Status |
|------|-----|--------|
| Home | `/Home.aspx` | ✅ Working |
| About | `/About.aspx` | ✅ Working |
| Students | `/Students.aspx` | ✅ Working |
| Courses | `/Courses.aspx` | ✅ Working |
| Instructors | `/Instructors.aspx` | ✅ Working |

## Screenshot Inventory

All screenshots captured from the running application are stored in `dev-docs/contoso-screenshots/`:

| Screenshot | Page | Key Controls Observed |
|-----------|------|-----------------------|
| `home.png` | Home.aspx | Navigation menu, welcome content |
| `about.png` | About.aspx | **GridView** — enrollment statistics (11 date rows) |
| `students.png` | Students.aspx | **GridView** (11 students), **DetailsView** (Add Student form), AutoCompleteExtender search |
| `courses.png` | Courses.aspx | **DropDownList** (department filter), AutoCompleteExtender name search, empty-state GridView |
| `instructors.png` | Instructors.aspx | **GridView** (7 instructors with sortable columns) |

## Issues Encountered & Fixes

### 1. Duplicate AssemblyVersion Attributes

**Problem:** The repo-root `Directory.Build.props` injects `Nerdbank.GitVersioning 3.9.50` into all projects. For this legacy .NET Framework 4.5.2 project with a manual `AssemblyInfo.cs`, the auto-generated attributes conflict with the hand-coded ones.

**Fix:** Created an empty `samples/ContosoUniversity/Directory.Build.props` to block inheritance from the repo root.

### 2. Missing AjaxControlToolkit Assembly

**Problem:** The `.csproj` HintPath pointed to a non-existent `Documents\ASP.NET AJAX Control Toolkit\` folder.

**Fix:** Installed `AjaxControlToolkit 16.1.1` via NuGet and updated the HintPath in the `.csproj` to `packages\AjaxControlToolkit.16.1.1.0\lib\net40\AjaxControlToolkit.dll`.

### 3. Connection String Update

**Problem:** Original `Web.config` connection strings referenced `.\SQLEXPRESS`, which is not available on the build machine.

**Fix:** Updated both connection strings (`SchoolContext` and `DefaultConnection`) to use `(localdb)\MSSQLLocalDB`.

### 4. Database Version Upgrade

**Problem:** The shipped `.mdf` is from an older SQL Server version (internal version 782). The original `.ldf` log file was not included.

**Fix:** Used `FOR ATTACH_REBUILD_LOG` which upgrades the data file and creates a fresh log file automatically.

## Project Technical Details

- **Target Framework:** .NET Framework 4.5.2
- **ORM:** Entity Framework 6.1.3 (Database-First, `.edmx` model)
- **AJAX Toolkit:** AjaxControlToolkit 16.1.1
- **Key Controls:** GridView, DetailsView, DropDownList, AutoCompleteExtender, Menu
- **Master Page:** `Site.Master` provides consistent navigation across all pages
