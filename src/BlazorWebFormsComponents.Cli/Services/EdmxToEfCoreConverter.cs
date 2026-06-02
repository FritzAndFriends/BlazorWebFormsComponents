using System.Text;
using System.Xml.Linq;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Services;

public sealed class EdmxToEfCoreConverter
{
    private static readonly Dictionary<string, string> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Binary"] = "byte[]",
        ["Boolean"] = "bool",
        ["Byte"] = "byte",
        ["DateTime"] = "DateTime",
        ["DateTimeOffset"] = "DateTimeOffset",
        ["Decimal"] = "decimal",
        ["Double"] = "double",
        ["Guid"] = "Guid",
        ["Int16"] = "short",
        ["Int32"] = "int",
        ["Int64"] = "long",
        ["SByte"] = "sbyte",
        ["Single"] = "float",
        ["String"] = "string",
        ["Time"] = "TimeSpan"
    };

    private static readonly HashSet<string> ValueTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "bool", "byte", "short", "int", "long", "sbyte", "DateTime", "DateTimeOffset", "decimal", "double", "float", "Guid", "TimeSpan"
    };

    public async Task<HashSet<string>> ConvertAsync(string sourcePath, string outputPath, string projectName, bool dryRun, MigrationReport report)
    {
        var normalizedSourcePath = Path.GetFullPath(sourcePath);
        var normalizedOutputPath = Path.GetFullPath(outputPath);
        var excludedSourceFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var edmxFiles = Directory.EnumerateFiles(sourcePath, "*.edmx", SearchOption.AllDirectories)
            .Where(file => Path.GetDirectoryName(file)?.EndsWith("Models", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        if (edmxFiles.Count == 0)
            return excludedSourceFiles;

        if (dryRun)
        {
            report.Warnings.Add("EDMX conversion is skipped in --dry-run mode.");
            return excludedSourceFiles;
        }

        foreach (var edmxFile in edmxFiles)
        {
            var normalizedEdmxPath = Path.GetFullPath(edmxFile);
            var relativeDir = Path.GetDirectoryName(Path.GetRelativePath(normalizedSourcePath, normalizedEdmxPath)) ?? string.Empty;
            var targetDirectory = Path.Combine(normalizedOutputPath, relativeDir);
            var result = await ConvertAsync(new EdmxConversionOptions(normalizedEdmxPath, targetDirectory, $"{projectName}.Models"));

            if (!result.Success)
            {
                report.Warnings.Add($"EDMX conversion failed for {Path.GetFileName(edmxFile)}: {result.ErrorMessage}");
                continue;
            }

            var stem = Path.GetFileNameWithoutExtension(normalizedEdmxPath);
            var sourceModelsDirectory = Path.GetDirectoryName(normalizedEdmxPath)!;
            var excludedForModel = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                Path.Combine(sourceModelsDirectory, $"{stem}.cs"),
                Path.Combine(sourceModelsDirectory, $"{stem}.Context.cs"),
                Path.Combine(sourceModelsDirectory, $"{stem}.Designer.cs")
            };

            foreach (var generatedFile in result.GeneratedFiles)
            {
                var sourceCounterpart = Path.Combine(sourceModelsDirectory, Path.GetFileName(generatedFile));
                if (File.Exists(sourceCounterpart))
                    excludedForModel.Add(Path.GetFullPath(sourceCounterpart));
            }

            foreach (var excludedFile in excludedForModel)
            {
                excludedSourceFiles.Add(Path.GetFullPath(excludedFile));
                DeleteStaleExcludedOutput(targetDirectory, excludedFile, result.GeneratedFiles);
            }

            report.AddManualItem(Path.GetRelativePath(normalizedSourcePath, normalizedEdmxPath), 0, "EDMX", "EDMX converted to EF Core entities and DbContext — verify generated relationships and configuration.");
        }

        return excludedSourceFiles;
    }

    private static void DeleteStaleExcludedOutput(string targetDirectory, string excludedSourceFile, IReadOnlyList<string> generatedFiles)
    {
        var outputCandidate = Path.Combine(targetDirectory, Path.GetFileName(excludedSourceFile));
        if (!File.Exists(outputCandidate))
            return;

        var generatedNames = new HashSet<string>(generatedFiles.Select(Path.GetFileName), StringComparer.OrdinalIgnoreCase);
        if (generatedNames.Contains(Path.GetFileName(outputCandidate)))
            return;

        File.Delete(outputCandidate);
    }

    public async Task<EdmxConversionResult> ConvertAsync(EdmxConversionOptions options, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(options.EdmxPath))
            return EdmxConversionResult.Failed($"EDMX file not found: {options.EdmxPath}");

        XDocument document;
        try
        {
            document = XDocument.Load(options.EdmxPath);
        }
        catch (Exception ex)
        {
            return EdmxConversionResult.Failed($"Unable to read EDMX XML: {ex.Message}");
        }

        EdmxModel model;
        try
        {
            model = Parse(document, options.Namespace, options.EdmxPath);
        }
        catch (Exception ex)
        {
            return EdmxConversionResult.Failed(ex.Message);
        }

        Directory.CreateDirectory(options.OutputPath);
        var generatedFiles = new List<string>();

        foreach (var entity in model.Entities.Values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filePath = Path.Combine(options.OutputPath, $"{entity.Name}.cs");
            await File.WriteAllTextAsync(filePath, GenerateEntityCode(model.Namespace, entity), cancellationToken);
            generatedFiles.Add(filePath);
        }

        var dbContextPath = Path.Combine(options.OutputPath, $"{model.ContextName}.cs");
        await File.WriteAllTextAsync(dbContextPath, GenerateDbContextCode(model), cancellationToken);
        generatedFiles.Add(dbContextPath);

        var relationshipCount = model.Associations.Values.Count(association => association.ForeignKeyProperties.Count > 0);
        return new EdmxConversionResult(true, null, model.Entities.Count, true, relationshipCount, generatedFiles);
    }

    private static EdmxModel Parse(XDocument document, string? requestedNamespace, string edmxPath)
    {
        var conceptualSchema = document
            .Descendants()
            .FirstOrDefault(element => element.Name.LocalName == "ConceptualModels")?
            .Elements()
            .FirstOrDefault(element => element.Name.LocalName == "Schema");
        if (conceptualSchema is null)
            throw new InvalidOperationException("The EDMX file does not contain conceptual schema metadata.");

        var mappingSchema = document
            .Descendants()
            .FirstOrDefault(element => element.Name.LocalName == "Mappings")?
            .Descendants()
            .FirstOrDefault(element => element.Name.LocalName == "Mapping");

        var entityToTable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var propertyToColumn = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (mappingSchema is not null)
        {
            foreach (var entitySetMapping in mappingSchema.Descendants().Where(element => element.Name.LocalName == "EntitySetMapping"))
            {
                var entityTypeMapping = entitySetMapping.Elements().FirstOrDefault(element => element.Name.LocalName == "EntityTypeMapping");
                var mappingFragment = entityTypeMapping?.Elements().FirstOrDefault(element => element.Name.LocalName == "MappingFragment");
                if (entityTypeMapping is null || mappingFragment is null)
                    continue;

                var typeName = StripNamespace(UnwrapIsTypeOf(entityTypeMapping.Attribute("TypeName")?.Value));
                if (string.IsNullOrWhiteSpace(typeName))
                    continue;

                var storeEntitySet = mappingFragment.Attribute("StoreEntitySet")?.Value;
                if (!string.IsNullOrWhiteSpace(storeEntitySet))
                    entityToTable[typeName] = storeEntitySet;

                foreach (var scalarProperty in mappingFragment.Elements().Where(element => element.Name.LocalName == "ScalarProperty"))
                {
                    var propertyName = scalarProperty.Attribute("Name")?.Value;
                    var columnName = scalarProperty.Attribute("ColumnName")?.Value;
                    if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(columnName))
                        continue;

                    propertyToColumn[$"{typeName}.{propertyName}"] = columnName;
                }
            }
        }

        var container = conceptualSchema.Elements().FirstOrDefault(element => element.Name.LocalName == "EntityContainer");
        var conceptualNamespace = conceptualSchema.Attribute("Namespace")?.Value;
        var modelNamespace = string.IsNullOrWhiteSpace(requestedNamespace)
            ? conceptualNamespace ?? $"{Path.GetFileNameWithoutExtension(edmxPath)}.Models"
            : requestedNamespace;
        var contextName = container?.Attribute("Name")?.Value ?? $"{Path.GetFileNameWithoutExtension(edmxPath)}Context";

        var entitySets = container?
            .Elements()
            .Where(element => element.Name.LocalName == "EntitySet")
            .Select(element => new EntitySetInfo(
                StripNamespace(element.Attribute("EntityType")?.Value) ?? string.Empty,
                element.Attribute("Name")?.Value ?? string.Empty))
            .Where(entitySet => !string.IsNullOrWhiteSpace(entitySet.EntityType) && !string.IsNullOrWhiteSpace(entitySet.Name))
            .ToDictionary(entitySet => entitySet.EntityType, entitySet => entitySet, StringComparer.OrdinalIgnoreCase)
            ?? new Dictionary<string, EntitySetInfo>(StringComparer.OrdinalIgnoreCase);

        var associations = conceptualSchema.Elements()
            .Where(element => element.Name.LocalName == "Association")
            .Select(ParseAssociation)
            .ToDictionary(association => association.Name, association => association, StringComparer.OrdinalIgnoreCase);

        var entities = conceptualSchema.Elements()
            .Where(element => element.Name.LocalName == "EntityType")
            .Select(entity => ParseEntity(entity, associations, entityToTable, propertyToColumn))
            .ToDictionary(entity => entity.Name, entity => entity, StringComparer.OrdinalIgnoreCase);

        if (entities.Count == 0)
            throw new InvalidOperationException("The EDMX file does not define any entity types.");

        return new EdmxModel(modelNamespace!, contextName, entities, entitySets, associations);
    }

    private static EntityInfo ParseEntity(
        XElement entityElement,
        IReadOnlyDictionary<string, AssociationInfo> associations,
        IReadOnlyDictionary<string, string> entityToTable,
        IReadOnlyDictionary<string, string> propertyToColumn)
    {
        var entityName = entityElement.Attribute("Name")?.Value;
        if (string.IsNullOrWhiteSpace(entityName))
            throw new InvalidOperationException("Encountered an entity type without a name.");

        var keyProperties = entityElement
            .Elements()
            .FirstOrDefault(element => element.Name.LocalName == "Key")?
            .Elements()
            .Where(element => element.Name.LocalName == "PropertyRef")
            .Select(element => element.Attribute("Name")?.Value ?? string.Empty)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? [];

        var properties = entityElement.Elements()
            .Where(element => element.Name.LocalName == "Property")
            .Select(property => ParseProperty(entityName, property, keyProperties, propertyToColumn))
            .ToList();

        var navigationProperties = entityElement.Elements()
            .Where(element => element.Name.LocalName == "NavigationProperty")
            .Select(navigation => ParseNavigationProperty(navigation, associations))
            .ToList();

        entityToTable.TryGetValue(entityName, out var tableName);
        return new EntityInfo(entityName, tableName, properties, navigationProperties);
    }

    private static PropertyInfo ParseProperty(
        string entityName,
        XElement propertyElement,
        IReadOnlySet<string> keyProperties,
        IReadOnlyDictionary<string, string> propertyToColumn)
    {
        var propertyName = propertyElement.Attribute("Name")?.Value;
        var edmTypeName = StripNamespace(propertyElement.Attribute("Type")?.Value);
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(edmTypeName))
            throw new InvalidOperationException($"Entity '{entityName}' contains a property with incomplete metadata.");

        var nullableAttribute = propertyElement.Attribute("Nullable")?.Value;
        var isNullable = !string.Equals(nullableAttribute, "false", StringComparison.OrdinalIgnoreCase);
        var maxLength = propertyElement.Attribute("MaxLength")?.Value;
        var storeGeneratedPattern = propertyElement.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "StoreGeneratedPattern")?.Value;

        var csType = TypeMap.TryGetValue(edmTypeName, out var mappedType) ? mappedType : edmTypeName;
        if (isNullable && ValueTypes.Contains(csType))
            csType += "?";

        propertyToColumn.TryGetValue($"{entityName}.{propertyName}", out var columnName);
        return new PropertyInfo(
            propertyName,
            edmTypeName,
            csType,
            isNullable,
            maxLength,
            keyProperties.Contains(propertyName),
            string.Equals(storeGeneratedPattern, "Identity", StringComparison.OrdinalIgnoreCase),
            string.Equals(storeGeneratedPattern, "Computed", StringComparison.OrdinalIgnoreCase),
            columnName);
    }

    private static NavigationPropertyInfo ParseNavigationProperty(XElement navigationElement, IReadOnlyDictionary<string, AssociationInfo> associations)
    {
        var navigationName = navigationElement.Attribute("Name")?.Value;
        var associationName = StripNamespace(navigationElement.Attribute("Relationship")?.Value);
        var fromRole = navigationElement.Attribute("FromRole")?.Value;
        var toRole = navigationElement.Attribute("ToRole")?.Value;

        if (string.IsNullOrWhiteSpace(navigationName)
            || string.IsNullOrWhiteSpace(associationName)
            || string.IsNullOrWhiteSpace(fromRole)
            || string.IsNullOrWhiteSpace(toRole))
        {
            throw new InvalidOperationException("Encountered a navigation property with incomplete metadata.");
        }

        if (!associations.TryGetValue(associationName, out var association))
            throw new InvalidOperationException($"Association '{associationName}' referenced by navigation property '{navigationName}' was not found.");
        if (!association.Ends.TryGetValue(toRole, out var targetEnd))
            throw new InvalidOperationException($"Association end '{toRole}' referenced by navigation property '{navigationName}' was not found.");

        return new NavigationPropertyInfo(navigationName, targetEnd.Type, targetEnd.Multiplicity == "*", associationName, fromRole, toRole);
    }

    private static AssociationInfo ParseAssociation(XElement associationElement)
    {
        var associationName = associationElement.Attribute("Name")?.Value;
        if (string.IsNullOrWhiteSpace(associationName))
            throw new InvalidOperationException("Encountered an association without a name.");

        var ends = associationElement.Elements()
            .Where(element => element.Name.LocalName == "End")
            .Select(end =>
            {
                var role = end.Attribute("Role")?.Value ?? string.Empty;
                var type = StripNamespace(end.Attribute("Type")?.Value) ?? string.Empty;
                var multiplicity = end.Attribute("Multiplicity")?.Value ?? string.Empty;
                var onDelete = end.Elements().FirstOrDefault(element => element.Name.LocalName == "OnDelete")?.Attribute("Action")?.Value;
                return new AssociationEndInfo(role, type, multiplicity, onDelete);
            })
            .Where(end => !string.IsNullOrWhiteSpace(end.Role))
            .ToDictionary(end => end.Role, end => end, StringComparer.OrdinalIgnoreCase);

        var referentialConstraint = associationElement.Elements().FirstOrDefault(element => element.Name.LocalName == "ReferentialConstraint");
        var principalRole = referentialConstraint?.Elements().FirstOrDefault(element => element.Name.LocalName == "Principal")?.Attribute("Role")?.Value;
        var dependentElement = referentialConstraint?.Elements().FirstOrDefault(element => element.Name.LocalName == "Dependent");
        var dependentRole = dependentElement?.Attribute("Role")?.Value;
        var foreignKeyProperties = dependentElement?
            .Elements()
            .Where(element => element.Name.LocalName == "PropertyRef")
            .Select(element => element.Attribute("Name")?.Value ?? string.Empty)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList() ?? [];

        return new AssociationInfo(associationName, ends, principalRole, dependentRole, foreignKeyProperties);
    }

    private static string GenerateEntityCode(string modelNamespace, EntityInfo entity)
    {
        var builder = new StringBuilder();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.ComponentModel.DataAnnotations;");
        builder.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
        builder.AppendLine();
        builder.AppendLine($"namespace {modelNamespace};");
        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(entity.TableName) && !string.Equals(entity.TableName, entity.Name, StringComparison.OrdinalIgnoreCase))
            builder.AppendLine($"[Table(\"{entity.TableName}\")]");

        builder.AppendLine($"public partial class {entity.Name}");
        builder.AppendLine("{");

        foreach (var property in entity.Properties)
        {
            foreach (var annotation in GetPropertyAnnotations(property))
                builder.AppendLine($"    {annotation}");

            builder.AppendLine($"    public {property.CsType} {property.Name} {{ get; set; }}");
            builder.AppendLine();
        }

        foreach (var navigation in entity.NavigationProperties)
        {
            var typeName = navigation.IsCollection
                ? $"ICollection<{navigation.TargetType}>"
                : $"{navigation.TargetType}?";
            var initializer = navigation.IsCollection ? " = [];" : string.Empty;
            builder.AppendLine($"    public virtual {typeName} {navigation.Name} {{ get; set; }}{initializer}");
            builder.AppendLine();
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private static IEnumerable<string> GetPropertyAnnotations(PropertyInfo property)
    {
        if (property.IsKey)
            yield return "[Key]";
        if (property.IsIdentity)
            yield return "[DatabaseGenerated(DatabaseGeneratedOption.Identity)]";
        if (property.IsComputed)
            yield return "[DatabaseGenerated(DatabaseGeneratedOption.Computed)]";
        if (!property.IsNullable && string.Equals(property.EdmType, "String", StringComparison.OrdinalIgnoreCase))
            yield return "[Required]";
        if (!string.IsNullOrWhiteSpace(property.MaxLength)
            && !string.Equals(property.MaxLength, "Max", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(property.MaxLength, out var maxLength))
        {
            yield return $"[MaxLength({maxLength})]";
        }
        if (!string.IsNullOrWhiteSpace(property.ColumnName)
            && !string.Equals(property.ColumnName, property.Name, StringComparison.OrdinalIgnoreCase))
        {
            yield return $"[Column(\"{property.ColumnName}\")]";
        }
    }

    private static string GenerateDbContextCode(EdmxModel model)
    {
        var builder = new StringBuilder();
        builder.AppendLine("using Microsoft.EntityFrameworkCore;");
        builder.AppendLine();
        builder.AppendLine($"namespace {model.Namespace};");
        builder.AppendLine();
        builder.AppendLine($"public partial class {model.ContextName} : DbContext");
        builder.AppendLine("{");
        builder.AppendLine($"    public {model.ContextName}(DbContextOptions<{model.ContextName}> options) : base(options)");
        builder.AppendLine("    {");
        builder.AppendLine("    }");
        builder.AppendLine();

        foreach (var entitySet in model.EntitySets.Values.OrderBy(entitySet => entitySet.Name, StringComparer.OrdinalIgnoreCase))
            builder.AppendLine($"    public DbSet<{entitySet.EntityType}> {entitySet.Name} {{ get; set; }} = null!;");

        builder.AppendLine();
        builder.AppendLine("    protected override void OnModelCreating(ModelBuilder modelBuilder)");
        builder.AppendLine("    {");
        builder.AppendLine("        base.OnModelCreating(modelBuilder);");
        builder.AppendLine();

        foreach (var entity in model.Entities.Values.OrderBy(entity => entity.Name, StringComparer.OrdinalIgnoreCase))
        {
            var entityLines = BuildEntityModelConfiguration(model, entity);
            if (entityLines.Count == 0)
                continue;

            builder.AppendLine($"        modelBuilder.Entity<{entity.Name}>(entity =>");
            builder.AppendLine("        {");
            foreach (var line in entityLines)
                builder.AppendLine($"            {line}");
            builder.AppendLine("        });");
            builder.AppendLine();
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static List<string> BuildEntityModelConfiguration(EdmxModel model, EntityInfo entity)
    {
        var lines = new List<string>();

        var keyProperties = entity.Properties.Where(property => property.IsKey).Select(property => property.Name).ToList();
        if (keyProperties.Count == 1)
        {
            // Emit explicit HasKey() when the single PK name does not follow EF Core conventions
            // (i.e., not "Id" and not "{EntityName}Id" case-insensitively).
            // EF Core discovers conventional keys automatically; non-conventional names need
            // an explicit HasKey() call even when [Key] data annotation is also present.
            var keyName = keyProperties[0];
            var isConventional = string.Equals(keyName, "Id", StringComparison.OrdinalIgnoreCase)
                || string.Equals(keyName, $"{entity.Name}Id", StringComparison.OrdinalIgnoreCase);
            if (!isConventional)
                lines.Add($"entity.HasKey(e => e.{keyName});");
        }
        else if (keyProperties.Count > 1)
        {
            lines.Add($"entity.HasKey(e => new {{ {string.Join(", ", keyProperties.Select(name => $"e.{name}"))} }});");
        }

        if (!string.IsNullOrWhiteSpace(entity.TableName) && !string.Equals(entity.TableName, entity.Name, StringComparison.OrdinalIgnoreCase))
            lines.Add($"entity.ToTable(\"{entity.TableName}\");");

        foreach (var association in model.Associations.Values.Where(association =>
                     string.Equals(association.DependentEntity, entity.Name, StringComparison.OrdinalIgnoreCase)
                     && association.ForeignKeyProperties.Count > 0))
        {
            var dependentNavigation = entity.NavigationProperties.FirstOrDefault(navigation =>
                string.Equals(navigation.AssociationName, association.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(navigation.ToRole, association.PrincipalRole, StringComparison.OrdinalIgnoreCase));
            model.Entities.TryGetValue(association.PrincipalEntity, out var principalEntity);
            var principalNavigation = principalEntity?.NavigationProperties.FirstOrDefault(navigation =>
                string.Equals(navigation.AssociationName, association.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(navigation.ToRole, association.DependentRole, StringComparison.OrdinalIgnoreCase));

            var chain = new List<string>
            {
                dependentNavigation is null
                    ? $"entity.HasOne<{association.PrincipalEntity}>()"
                    : $"entity.HasOne(d => d.{dependentNavigation.Name})"
            };

            if (principalNavigation is null)
            {
                chain.Add(association.IsPrincipalCollection ? ".WithMany()" : ".WithOne()");
            }
            else
            {
                chain.Add(principalNavigation.IsCollection
                    ? $".WithMany(p => p.{principalNavigation.Name})"
                    : $".WithOne(p => p.{principalNavigation.Name})");
            }

            if (association.ForeignKeyProperties.Count == 1)
            {
                chain.Add($".HasForeignKey(d => d.{association.ForeignKeyProperties[0]})");
            }
            else
            {
                chain.Add($".HasForeignKey(d => new {{ {string.Join(", ", association.ForeignKeyProperties.Select(name => $"d.{name}"))} }})");
            }

            var deleteBehavior = association.DeleteBehavior;
            if (!string.IsNullOrWhiteSpace(deleteBehavior))
                chain.Add($".OnDelete({deleteBehavior})");

            chain[^1] += ";";
            lines.Add(chain[0]);
            lines.AddRange(chain.Skip(1).Select(part => $"    {part}"));
        }

        return lines;
    }

    private static string? StripNamespace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var cleanValue = value.Trim();
        var lastDot = cleanValue.LastIndexOf('.');
        return lastDot >= 0 ? cleanValue[(lastDot + 1)..] : cleanValue;
    }

    private static string? UnwrapIsTypeOf(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        const string prefix = "IsTypeOf(";
        if (!value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !value.EndsWith(')'))
            return value;

        return value[prefix.Length..^1];
    }

    private sealed record EdmxModel(
        string Namespace,
        string ContextName,
        IReadOnlyDictionary<string, EntityInfo> Entities,
        IReadOnlyDictionary<string, EntitySetInfo> EntitySets,
        IReadOnlyDictionary<string, AssociationInfo> Associations);

    private sealed record EntityInfo(
        string Name,
        string? TableName,
        IReadOnlyList<PropertyInfo> Properties,
        IReadOnlyList<NavigationPropertyInfo> NavigationProperties);

    private sealed record PropertyInfo(
        string Name,
        string EdmType,
        string CsType,
        bool IsNullable,
        string? MaxLength,
        bool IsKey,
        bool IsIdentity,
        bool IsComputed,
        string? ColumnName);

    private sealed record NavigationPropertyInfo(
        string Name,
        string TargetType,
        bool IsCollection,
        string AssociationName,
        string FromRole,
        string ToRole);

    private sealed record EntitySetInfo(string EntityType, string Name);

    private sealed record AssociationEndInfo(string Role, string Type, string Multiplicity, string? OnDelete);

    private sealed record AssociationInfo(
        string Name,
        IReadOnlyDictionary<string, AssociationEndInfo> Ends,
        string? PrincipalRole,
        string? DependentRole,
        IReadOnlyList<string> ForeignKeyProperties)
    {
        public string PrincipalEntity => PrincipalRole is not null && Ends.TryGetValue(PrincipalRole, out var principalEnd)
            ? principalEnd.Type
            : string.Empty;

        public string DependentEntity => DependentRole is not null && Ends.TryGetValue(DependentRole, out var dependentEnd)
            ? dependentEnd.Type
            : string.Empty;

        public bool IsPrincipalCollection => PrincipalRole is not null && Ends.TryGetValue(PrincipalRole, out var principalEnd)
            && principalEnd.Multiplicity == "*";

        public string? DeleteBehavior => PrincipalRole is not null
            && Ends.TryGetValue(PrincipalRole, out var principalEnd)
            && !string.IsNullOrWhiteSpace(principalEnd.OnDelete)
            ? principalEnd.OnDelete.Equals("Cascade", StringComparison.OrdinalIgnoreCase)
                ? "DeleteBehavior.Cascade"
                : "DeleteBehavior.Restrict"
            : null;
    }
}

public sealed record EdmxConversionOptions(string EdmxPath, string OutputPath, string? Namespace);

public sealed record EdmxConversionResult(
    bool Success,
    string? ErrorMessage,
    int EntitiesGenerated,
    bool DbContextGenerated,
    int RelationshipsConfigured,
    IReadOnlyList<string> GeneratedFiles)
{
    public static EdmxConversionResult Failed(string message) => new(false, message, 0, false, 0, []);
}
