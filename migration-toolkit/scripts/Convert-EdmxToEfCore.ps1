<#
.SYNOPSIS
    Converts an Entity Framework 6 EDMX file to EF Core entity classes and DbContext.

.DESCRIPTION
    Parses SSDL/CSDL/C-S Mapping sections of an .edmx file to generate:
    - Entity .cs files with proper data annotations ([Key], [Required], [MaxLength], [Table], [Column], etc.)
    - A DbContext .cs file with OnModelCreating() FK relationships and cascade deletes.

.PARAMETER EdmxPath
    Path to the .edmx file.

.PARAMETER OutputPath
    Directory to write generated .cs files.

.PARAMETER Namespace
    C# namespace for generated files. Auto-detected from EDMX if not provided.

.EXAMPLE
    .\Convert-EdmxToEfCore.ps1 -EdmxPath "Models\Model1.edmx" -OutputPath ".\Models" -Namespace "ContosoUniversity.Models"
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)]
    [string]$EdmxPath,

    [Parameter(Mandatory)]
    [string]$OutputPath,

    [string]$Namespace
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# --- Validate input ---
if (-not (Test-Path $EdmxPath -PathType Leaf)) {
    throw "EDMX file not found: $EdmxPath"
}

# --- Load and parse EDMX XML ---
Write-Host "Parsing EDMX: $EdmxPath" -ForegroundColor Cyan
[xml]$edmx = Get-Content -Path $EdmxPath -Raw -Encoding UTF8

$nsManager = New-Object System.Xml.XmlNamespaceManager($edmx.NameTable)
$nsManager.AddNamespace('edmx', 'http://schemas.microsoft.com/ado/2009/11/edmx')
$nsManager.AddNamespace('ssdl', 'http://schemas.microsoft.com/ado/2009/11/edm/ssdl')
$nsManager.AddNamespace('edm', 'http://schemas.microsoft.com/ado/2009/11/edm')
$nsManager.AddNamespace('cs', 'http://schemas.microsoft.com/ado/2009/11/mapping/cs')
$nsManager.AddNamespace('annotation', 'http://schemas.microsoft.com/ado/2009/02/edm/annotation')
$nsManager.AddNamespace('store', 'http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator')

$annotationNs = 'http://schemas.microsoft.com/ado/2009/02/edm/annotation'

# --- EDM Type to C# Type Map ---
$typeMap = @{
    'Int32'          = 'int'
    'Int16'          = 'short'
    'Int64'          = 'long'
    'Byte'           = 'byte'
    'SByte'          = 'sbyte'
    'String'         = 'string'
    'DateTime'       = 'DateTime'
    'DateTimeOffset' = 'DateTimeOffset'
    'Decimal'        = 'decimal'
    'Double'         = 'double'
    'Single'         = 'float'
    'Boolean'        = 'bool'
    'Binary'         = 'byte[]'
    'Guid'           = 'Guid'
    'Time'           = 'TimeSpan'
}
$valueTypes = @('int', 'short', 'long', 'byte', 'sbyte', 'DateTime', 'DateTimeOffset', 'decimal', 'double', 'float', 'bool', 'Guid', 'TimeSpan')

# --- Tracking ---
$warnings = [System.Collections.Generic.List[string]]::new()
$entitiesGenerated = 0
$relationshipsConfigured = 0
$cascadeDeletesConfigured = 0

# =====================================================================
# SECTION 1: Parse C-S Mapping (entity name -> table name, property -> column)
# =====================================================================
$entityToTable = @{}
$propertyToColumn = @{}

$mappings = $edmx.SelectNodes('//edmx:Edmx/edmx:Runtime/edmx:Mappings/cs:Mapping/cs:EntityContainerMapping/cs:EntitySetMapping', $nsManager)
foreach ($mapping in $mappings) {
    $entityTypeMapping = $mapping.SelectSingleNode('cs:EntityTypeMapping', $nsManager)
    if (-not $entityTypeMapping) { continue }

    $typeName = $entityTypeMapping.GetAttribute('TypeName')
    $entityName = $typeName -replace '^.*\.', ''

    $fragment = $entityTypeMapping.SelectSingleNode('cs:MappingFragment', $nsManager)
    if (-not $fragment) { continue }

    $storeEntitySet = $fragment.GetAttribute('StoreEntitySet')
    $entityToTable[$entityName] = $storeEntitySet

    foreach ($sp in $fragment.SelectNodes('cs:ScalarProperty', $nsManager)) {
        $propName = $sp.GetAttribute('Name')
        $colName = $sp.GetAttribute('ColumnName')
        $propertyToColumn["$entityName.$propName"] = $colName
    }
}

# =====================================================================
# SECTION 2: Parse CSDL (Conceptual Schema)
# =====================================================================
$csdlSchema = $edmx.SelectSingleNode('//edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/edm:Schema', $nsManager)
if (-not $Namespace) {
    $Namespace = $csdlSchema.GetAttribute('Namespace')
    Write-Host "  Auto-detected namespace: $Namespace" -ForegroundColor Yellow
    $warnings.Add("Namespace auto-detected from EDMX as '$Namespace' - consider providing an explicit namespace")
}

# Container name = DbContext class name
$container = $csdlSchema.SelectSingleNode('edm:EntityContainer', $nsManager)
$contextName = $container.GetAttribute('Name')

# EntitySets (DbSet property names): entityTypeName -> entitySetName
$entitySets = [ordered]@{}
foreach ($es in $container.SelectNodes('edm:EntitySet', $nsManager)) {
    $esName = $es.GetAttribute('Name')
    $esType = $es.GetAttribute('EntityType') -replace '^.*\.', ''
    $entitySets[$esType] = $esName
}

# Parse Associations
$associations = @{}
foreach ($assoc in $csdlSchema.SelectNodes('edm:Association', $nsManager)) {
    $assocName = $assoc.GetAttribute('Name')
    $assocInfo = @{
        Name               = $assocName
        Ends               = @{}
        ForeignKeyProperties = @()
    }

    foreach ($end in $assoc.SelectNodes('edm:End', $nsManager)) {
        $role = $end.GetAttribute('Role')
        $type = $end.GetAttribute('Type') -replace '^.*\.', ''
        $multiplicity = $end.GetAttribute('Multiplicity')

        $onDelete = $end.SelectSingleNode('edm:OnDelete', $nsManager)
        $deleteAction = if ($onDelete) { $onDelete.GetAttribute('Action') } else { $null }

        $assocInfo.Ends[$role] = @{
            Type         = $type
            Multiplicity = $multiplicity
            OnDelete     = $deleteAction
        }
    }

    $refConstraint = $assoc.SelectSingleNode('edm:ReferentialConstraint', $nsManager)
    if ($refConstraint) {
        $principal = $refConstraint.SelectSingleNode('edm:Principal', $nsManager)
        $dependent = $refConstraint.SelectSingleNode('edm:Dependent', $nsManager)

        $assocInfo['PrincipalRole'] = $principal.GetAttribute('Role')
        $assocInfo['DependentRole'] = $dependent.GetAttribute('Role')

        $fkProps = @()
        foreach ($propRef in $dependent.SelectNodes('edm:PropertyRef', $nsManager)) {
            $fkProps += $propRef.GetAttribute('Name')
        }
        $assocInfo['ForeignKeyProperties'] = $fkProps
    }

    $associations[$assocName] = $assocInfo
}

# Parse Entity Types
$entities = [ordered]@{}
foreach ($entity in $csdlSchema.SelectNodes('edm:EntityType', $nsManager)) {
    $entityName = $entity.GetAttribute('Name')

    # Key properties
    $keyProps = @()
    foreach ($keyRef in $entity.SelectNodes('edm:Key/edm:PropertyRef', $nsManager)) {
        $keyProps += $keyRef.GetAttribute('Name')
    }

    # Scalar properties
    $properties = @()
    foreach ($prop in $entity.SelectNodes('edm:Property', $nsManager)) {
        $propName = $prop.GetAttribute('Name')
        $edmType = $prop.GetAttribute('Type')
        $nullable = $prop.GetAttribute('Nullable')
        $maxLength = $prop.GetAttribute('MaxLength')
        $storeGenerated = $prop.GetAttribute('StoreGeneratedPattern', $annotationNs)

        $csType = if ($typeMap.ContainsKey($edmType)) { $typeMap[$edmType] } else { $edmType }

        $isNullable = ($nullable -ne 'false')
        if ($isNullable -and $csType -in $valueTypes) {
            $csType = "$csType?"
        }

        $columnName = $propertyToColumn["$entityName.$propName"]

        $properties += @{
            Name       = $propName
            EdmType    = $edmType
            CsType     = $csType
            IsNullable = $isNullable
            MaxLength  = $maxLength
            IsKey      = ($propName -in $keyProps)
            IsIdentity = ($storeGenerated -eq 'Identity')
            IsComputed = ($storeGenerated -eq 'Computed')
            ColumnName = $columnName
        }
    }

    # Navigation properties
    $navProperties = @()
    foreach ($nav in $entity.SelectNodes('edm:NavigationProperty', $nsManager)) {
        $navName = $nav.GetAttribute('Name')
        $relationship = ($nav.GetAttribute('Relationship')) -replace '^.*\.', ''
        $fromRole = $nav.GetAttribute('FromRole')
        $toRole = $nav.GetAttribute('ToRole')

        $assocInfo = $associations[$relationship]
        $targetEnd = $assocInfo.Ends[$toRole]
        $isCollection = ($targetEnd.Multiplicity -eq '*')

        $navProperties += @{
            Name            = $navName
            TargetType      = $targetEnd.Type
            IsCollection    = $isCollection
            AssociationName = $relationship
            FromRole        = $fromRole
            ToRole          = $toRole
        }
    }

    $entities[$entityName] = @{
        Name          = $entityName
        TableName     = $entityToTable[$entityName]
        KeyProperties = $keyProps
        Properties    = $properties
        NavProperties = $navProperties
    }
}

Write-Host "  Found $($entities.Count) entities, $($associations.Count) associations" -ForegroundColor Cyan

# --- Ensure output directory exists ---
if (-not (Test-Path $OutputPath)) {
    if ($PSCmdlet.ShouldProcess($OutputPath, 'Create output directory')) {
        New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    }
}

# =====================================================================
# SECTION 3: Generate Entity .cs Files
# =====================================================================
foreach ($entityName in $entities.Keys) {
    $entityInfo = $entities[$entityName]
    $fileName = "$entityName.cs"
    $filePath = Join-Path $OutputPath $fileName

    # Skip if file already exists
    if (Test-Path $filePath) {
        Write-Host "  Skipping $fileName (already exists)" -ForegroundColor Yellow
        $warnings.Add("Skipped generating $fileName - file already exists")
        continue
    }

    if (-not $PSCmdlet.ShouldProcess($filePath, 'Generate entity file')) { continue }

    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine('using System;')
    [void]$sb.AppendLine('using System.Collections.Generic;')
    [void]$sb.AppendLine('using System.ComponentModel.DataAnnotations;')
    [void]$sb.AppendLine('using System.ComponentModel.DataAnnotations.Schema;')
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("namespace $Namespace")
    [void]$sb.AppendLine('{')

    # [Table] attribute when entity name differs from table name
    $tableName = $entityInfo.TableName
    if ($tableName -and $tableName -ne $entityName) {
        [void]$sb.AppendLine("    [Table(`"$tableName`")]")
    }

    [void]$sb.AppendLine("    public class $entityName")
    [void]$sb.AppendLine('    {')

    # Scalar properties
    foreach ($prop in $entityInfo.Properties) {
        $annotations = @()

        if ($prop.IsKey) {
            $annotations += '        [Key]'
        }
        if ($prop.IsIdentity) {
            $annotations += '        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]'
        }
        # FIX 5: Add Computed annotation for StoreGeneratedPattern="Computed"
        if ($prop.IsComputed) {
            $annotations += '        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]'
        }
        if (-not $prop.IsNullable -and $prop.EdmType -eq 'String') {
            $annotations += '        [Required]'
        }
        if ($prop.MaxLength -and $prop.MaxLength -ne 'Max') {
            $annotations += "        [MaxLength($($prop.MaxLength))]"
        }
        if ($prop.ColumnName -and $prop.ColumnName -ne $prop.Name) {
            $annotations += "        [Column(`"$($prop.ColumnName)`")]"
        }

        foreach ($ann in $annotations) {
            [void]$sb.AppendLine($ann)
        }

        [void]$sb.AppendLine("        public $($prop.CsType) $($prop.Name) { get; set; }")
        [void]$sb.AppendLine()
    }

    # Navigation properties
    foreach ($nav in $entityInfo.NavProperties) {
        if ($nav.IsCollection) {
            [void]$sb.AppendLine("        public virtual ICollection<$($nav.TargetType)> $($nav.Name) { get; set; }")
        }
        else {
            [void]$sb.AppendLine("        public virtual $($nav.TargetType) $($nav.Name) { get; set; }")
        }
        [void]$sb.AppendLine()
    }

    [void]$sb.AppendLine('    }')
    [void]$sb.AppendLine('}')

    Set-Content -Path $filePath -Value $sb.ToString() -Encoding UTF8
    Write-Host "  Generated: $fileName" -ForegroundColor Green
    $entitiesGenerated++
}

# =====================================================================
# SECTION 4: Generate DbContext .cs File
# =====================================================================
$contextFileName = "$contextName.cs"
$contextFilePath = Join-Path $OutputPath $contextFileName

$dbContextGenerated = $false
if (Test-Path $contextFilePath) {
    Write-Host "  Skipping $contextFileName (already exists)" -ForegroundColor Yellow
    $warnings.Add("Skipped generating $contextFileName - file already exists")
}
elseif ($PSCmdlet.ShouldProcess($contextFilePath, 'Generate DbContext file')) {
    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine('using System;')
    [void]$sb.AppendLine('using Microsoft.EntityFrameworkCore;')
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("namespace $Namespace")
    [void]$sb.AppendLine('{')
    [void]$sb.AppendLine("    public class $contextName : DbContext")
    [void]$sb.AppendLine('    {')

    # Constructor
    [void]$sb.AppendLine("        public $contextName(DbContextOptions<$contextName> options) : base(options) { }")
    [void]$sb.AppendLine()

    # DbSet properties
    foreach ($esTypeName in $entitySets.Keys) {
        $esName = $entitySets[$esTypeName]
        [void]$sb.AppendLine("        public DbSet<$esTypeName> $esName { get; set; }")
    }
    [void]$sb.AppendLine()

    # OnModelCreating
    [void]$sb.AppendLine('        protected override void OnModelCreating(ModelBuilder modelBuilder)')
    [void]$sb.AppendLine('        {')

    # Group FK relationships by dependent entity
    $entityRelationships = @{}
    foreach ($assocName in $associations.Keys) {
        $assocInfo = $associations[$assocName]
        if (-not $assocInfo.ContainsKey('DependentRole')) { continue }

        $dependentRole = $assocInfo['DependentRole']
        $principalRole = $assocInfo['PrincipalRole']
        $dependentEnd = $assocInfo.Ends[$dependentRole]
        $principalEnd = $assocInfo.Ends[$principalRole]

        $dependentEntity = $dependentEnd.Type
        $principalEntity = $principalEnd.Type

        # Find nav property on dependent that points to principal
        $navToPrincipal = $null
        foreach ($nav in $entities[$dependentEntity].NavProperties) {
            if ($nav.AssociationName -eq $assocName -and $nav.ToRole -eq $principalRole) {
                $navToPrincipal = $nav
                break
            }
        }

        # Find nav property on principal that points to dependent
        $navToDependent = $null
        foreach ($nav in $entities[$principalEntity].NavProperties) {
            if ($nav.AssociationName -eq $assocName -and $nav.ToRole -eq $dependentRole) {
                $navToDependent = $nav
                break
            }
        }

        # Determine delete behavior
        $deleteAction = $principalEnd.OnDelete
        $deleteBehavior = switch ($deleteAction) {
            'Cascade' { 'DeleteBehavior.Cascade' }
            'None'    { 'DeleteBehavior.Restrict' }
            default   { $null }
        }

        $relInfo = @{
            AssociationName      = $assocName
            PrincipalEntity      = $principalEntity
            DependentEntity      = $dependentEntity
            NavToPrincipal       = $navToPrincipal
            NavToDependent       = $navToDependent
            ForeignKeyProperties = $assocInfo['ForeignKeyProperties']
            DeleteBehavior       = $deleteBehavior
        }

        if (-not $entityRelationships.ContainsKey($dependentEntity)) {
            $entityRelationships[$dependentEntity] = @()
        }
        $entityRelationships[$dependentEntity] += $relInfo
    }

    # Generate modelBuilder configuration per entity
    foreach ($entityName in $entities.Keys) {
        $entityInfo = $entities[$entityName]
        $tableName = $entityInfo.TableName
        $hasTableMapping = ($tableName -and $tableName -ne $entityName)
        $hasRelationships = $entityRelationships.ContainsKey($entityName)

        if (-not $hasTableMapping -and -not $hasRelationships) { continue }

        [void]$sb.AppendLine("            modelBuilder.Entity<$entityName>(entity =>")
        [void]$sb.AppendLine('            {')

        if ($hasTableMapping) {
            [void]$sb.AppendLine("                entity.ToTable(`"$tableName`");")
        }

        if ($hasRelationships) {
            foreach ($rel in $entityRelationships[$entityName]) {
                [void]$sb.AppendLine()

                $navPrincipalName = if ($rel.NavToPrincipal) { $rel.NavToPrincipal.Name } else { $null }
                $navDependentName = if ($rel.NavToDependent) { $rel.NavToDependent.Name } else { $null }

                # Build fluent chain parts
                $parts = [System.Collections.Generic.List[string]]::new()

                if ($navPrincipalName) {
                    $parts.Add("entity.HasOne(d => d.$navPrincipalName)")
                }
                else {
                    $parts.Add("entity.HasOne<$($rel.PrincipalEntity)>()")
                }

                if ($navDependentName) {
                    $parts.Add(".WithMany(p => p.$navDependentName)")
                }
                else {
                    $parts.Add('.WithMany()')
                }

                $fkProps = $rel.ForeignKeyProperties
                if ($fkProps.Count -eq 1) {
                    $parts.Add(".HasForeignKey(d => d.$($fkProps[0]))")
                }
                elseif ($fkProps.Count -gt 1) {
                    $fkList = ($fkProps | ForEach-Object { "d.$_" }) -join ', '
                    $parts.Add(".HasForeignKey(d => new { $fkList })")
                }

                if ($rel.DeleteBehavior) {
                    $parts.Add(".OnDelete($($rel.DeleteBehavior))")
                    $cascadeDeletesConfigured++
                }

                # Add semicolon to last part
                $parts[$parts.Count - 1] = $parts[$parts.Count - 1] + ';'

                # Write: first part at 16-space indent, subsequent parts at 20-space indent
                [void]$sb.AppendLine("                $($parts[0])")
                for ($i = 1; $i -lt $parts.Count; $i++) {
                    [void]$sb.AppendLine("                    $($parts[$i])")
                }

                $relationshipsConfigured++
            }
        }

        [void]$sb.AppendLine('            });')
        [void]$sb.AppendLine()
    }

    [void]$sb.AppendLine('        }')
    [void]$sb.AppendLine('    }')
    [void]$sb.AppendLine('}')

    Set-Content -Path $contextFilePath -Value $sb.ToString() -Encoding UTF8
    Write-Host "  Generated: $contextFileName" -ForegroundColor Green
    $dbContextGenerated = $true
}

# --- Return Summary ---
$summary = [PSCustomObject]@{
    EntitiesGenerated        = $entitiesGenerated
    DbContextGenerated       = $dbContextGenerated
    RelationshipsConfigured  = $relationshipsConfigured
    CascadeDeletesConfigured = $cascadeDeletesConfigured
    Warnings                 = $warnings.ToArray()
}

Write-Host "`nEDMX conversion complete: $entitiesGenerated entities, $relationshipsConfigured relationships, $cascadeDeletesConfigured cascade deletes" -ForegroundColor Green
if ($warnings.Count -gt 0) {
    Write-Host "Warnings:" -ForegroundColor Yellow
    foreach ($w in $warnings) {
        Write-Host "  - $w" -ForegroundColor Yellow
    }
}

return $summary
