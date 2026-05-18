using BlazorWebFormsComponents.Cli.Services;

namespace BlazorWebFormsComponents.Cli.Tests.Services;

public sealed class EdmxToEfCoreConverterTests : IDisposable
{
    private readonly string _testRoot = Path.Combine(AppContext.BaseDirectory, "TestOutput", nameof(EdmxToEfCoreConverterTests), Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
            Directory.Delete(_testRoot, recursive: true);
    }

    [Fact]
    public async Task ConvertAsync_SimpleEdmx_GeneratesEntitiesAndDbContext()
    {
        var sourceFile = CreateFile("simple", "Store.edmx", SampleEdmx);
        var outputPath = CreateDirectory("simple", "generated");

        var converter = new EdmxToEfCoreConverter();
        var result = await converter.ConvertAsync(new EdmxConversionOptions(sourceFile, outputPath, "Contoso.Models"));

        Assert.True(result.Success);
        Assert.Equal(3, result.EntitiesGenerated);
        Assert.True(result.DbContextGenerated);
        Assert.True(File.Exists(Path.Combine(outputPath, "Customer.cs")));
        Assert.True(File.Exists(Path.Combine(outputPath, "Order.cs")));
        Assert.True(File.Exists(Path.Combine(outputPath, "Product.cs")));
        Assert.True(File.Exists(Path.Combine(outputPath, "StoreContext.cs")));

        var customerCode = await File.ReadAllTextAsync(Path.Combine(outputPath, "Customer.cs"));
        Assert.Contains("namespace Contoso.Models;", customerCode);
        Assert.Contains("[Table(\"tblCustomers\")]", customerCode);
        Assert.Contains("[Column(\"CustomerId\")]", customerCode);
        Assert.Contains("public string Name { get; set; }", customerCode);

        var dbContextCode = await File.ReadAllTextAsync(Path.Combine(outputPath, "StoreContext.cs"));
        Assert.Contains("public DbSet<Customer> Customers", dbContextCode);
        Assert.Contains("public DbSet<Order> Orders", dbContextCode);
        Assert.Contains("public DbSet<Product> Products", dbContextCode);
    }

    [Fact]
    public async Task ConvertAsync_NavigationProperties_GeneratesRelationshipConfiguration()
    {
        var sourceFile = CreateFile("relationships", "Store.edmx", SampleEdmx);
        var outputPath = CreateDirectory("relationships", "generated");

        var converter = new EdmxToEfCoreConverter();
        var result = await converter.ConvertAsync(new EdmxConversionOptions(sourceFile, outputPath, "Contoso.Models"));

        Assert.True(result.Success);
        Assert.Equal(1, result.RelationshipsConfigured);

        var customerCode = await File.ReadAllTextAsync(Path.Combine(outputPath, "Customer.cs"));
        var orderCode = await File.ReadAllTextAsync(Path.Combine(outputPath, "Order.cs"));
        var dbContextCode = await File.ReadAllTextAsync(Path.Combine(outputPath, "StoreContext.cs"));

        Assert.Contains("public virtual ICollection<Order> Orders { get; set; } = [];", customerCode);
        Assert.Contains("public virtual Customer? Customer { get; set; }", orderCode);
        Assert.Contains("entity.HasOne(d => d.Customer)", dbContextCode);
        Assert.Contains(".WithMany(p => p.Orders)", dbContextCode);
        Assert.Contains(".HasForeignKey(d => d.CustomerId)", dbContextCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConvertAsync_MissingOrInvalidEdmx_ReturnsGracefulError(bool createInvalidFile)
    {
        var outputPath = CreateDirectory("invalid", createInvalidFile ? "bad" : "missing");
        var sourceFile = Path.Combine(outputPath, "Broken.edmx");
        if (createInvalidFile)
            await File.WriteAllTextAsync(sourceFile, "<not-valid>");

        var converter = new EdmxToEfCoreConverter();
        var result = await converter.ConvertAsync(new EdmxConversionOptions(sourceFile, outputPath, "Contoso.Models"));

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.GeneratedFiles);
        Assert.Contains(createInvalidFile ? "Unable to read EDMX XML" : "EDMX file not found", result.ErrorMessage!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ConvertAsync_Batch_ExcludesT4GeneratedSourceFiles()
    {
        // Verifies that after EDMX conversion, both the T4 companion files and any
        // source files that share names with generated EF Core outputs are excluded,
        // and stale legacy output artifacts are removed on rerun.
        var sourceRoot = CreateDirectory("batch-excl");
        var modelsDir = CreateDirectory("batch-excl", "Models");
        _ = CreateFile("batch-excl", Path.Combine("Models", "Model1.edmx"), SampleEdmx);
        var outputPath = CreateDirectory("batch-excl", "out");
        var outputModelsDir = CreateDirectory("batch-excl", "out", "Models");

        // Create the T4-generated companion files that should be excluded
        var t4Context = Path.Combine(modelsDir, "Model1.Context.cs");
        var t4Entities = Path.Combine(modelsDir, "Model1.cs");
        var t4Designer = Path.Combine(modelsDir, "Model1.Designer.cs");
        File.WriteAllText(t4Context, "// T4 context");
        File.WriteAllText(t4Entities, "// T4 entities");
        File.WriteAllText(t4Designer, "// T4 designer");

        // Create source entity files that would otherwise overwrite the generated EF Core output.
        var customerSource = Path.Combine(modelsDir, "Customer.cs");
        var orderSource = Path.Combine(modelsDir, "Order.cs");
        File.WriteAllText(customerSource, "// EF6 customer");
        File.WriteAllText(orderSource, "// EF6 order");

        // Seed stale legacy output to verify reruns clean it up.
        var staleOutputContext = Path.Combine(outputModelsDir, "Model1.Context.cs");
        File.WriteAllText(staleOutputContext, "// stale output");

        var converter = new EdmxToEfCoreConverter();
        var report = new BlazorWebFormsComponents.Cli.Pipeline.MigrationReport();
        var excluded = await converter.ConvertAsync(sourceRoot, outputPath, "StoreProject", dryRun: false, report);

        Assert.Contains(t4Context, excluded, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(t4Entities, excluded, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(t4Designer, excluded, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(customerSource, excluded, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(orderSource, excluded, StringComparer.OrdinalIgnoreCase);
        Assert.False(File.Exists(staleOutputContext));
    }

    [Fact]
    public async Task ConvertAsync_NonConventionalSingleKey_EmitsHasKeyInOnModelCreating()
    {
        // Reproduces the ContosoUniversity pattern: entity named "Cours" with key "CourseID".
        // EF Core convention looks for "Id" or "{TypeName}Id" (i.e., "CoursId"), so "CourseID"
        // is non-conventional and needs an explicit HasKey() call.
        var sourceFile = CreateFile("nonconventional-key", "School.edmx", NonConventionalKeyEdmx);
        var outputPath = CreateDirectory("nonconventional-key", "generated");

        var converter = new EdmxToEfCoreConverter();
        var result = await converter.ConvertAsync(new EdmxConversionOptions(sourceFile, outputPath, "School.Models"));

        Assert.True(result.Success);
        var dbContextCode = await File.ReadAllTextAsync(Path.Combine(outputPath, "SchoolContext.cs"));

        // Cours entity key "CourseID" is non-conventional → must have HasKey()
        Assert.Contains("entity.HasKey(e => e.CourseID)", dbContextCode);

        // Student entity key "StudentID" matches convention "{TypeName}Id" → no HasKey() needed
        Assert.DoesNotContain("entity.HasKey(e => e.StudentID)", dbContextCode);
    }

    [Fact]
    public async Task ConvertAsync_ConventionalIdKey_DoesNotEmitHasKey()
    {
        // Entities using plain "Id" as the PK should NOT get a HasKey() call —
        // EF Core discovers them by convention and the [Key] attribute is sufficient.
        var sourceFile = CreateFile("conventional-key", "Store.edmx", SampleEdmx);
        var outputPath = CreateDirectory("conventional-key", "generated");

        var converter = new EdmxToEfCoreConverter();
        var result = await converter.ConvertAsync(new EdmxConversionOptions(sourceFile, outputPath, "Contoso.Models"));

        Assert.True(result.Success);
        var dbContextCode = await File.ReadAllTextAsync(Path.Combine(outputPath, "StoreContext.cs"));

        // All entities in SampleEdmx use "Id" — no HasKey() calls should appear for single keys
        Assert.DoesNotContain("entity.HasKey(e => e.Id)", dbContextCode);
    }

    private string CreateDirectory(params string[] segments)
    {
        var path = Path.Combine([_testRoot, .. segments]);
        Directory.CreateDirectory(path);
        return path;
    }

    private string CreateFile(string folder, string fileName, string content)
    {
        var directory = CreateDirectory(folder);
        var filePath = Path.Combine(directory, fileName);
        File.WriteAllText(filePath, content);
        return filePath;
    }

    // EDMX fixture with non-conventional single keys:
    //   Cours entity uses CourseID (non-conventional — entity is "Cours", not "Course")
    //   Student entity uses StudentID (conventional — matches "StudentId" case-insensitively)
    private const string NonConventionalKeyEdmx = """
        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="3.0" xmlns:edmx="http://schemas.microsoft.com/ado/2009/11/edmx">
          <edmx:Runtime>
            <edmx:ConceptualModels>
              <Schema Namespace="SchoolModel" xmlns="http://schemas.microsoft.com/ado/2009/11/edm">
                <EntityType Name="Cours">
                  <Key>
                    <PropertyRef Name="CourseID" />
                  </Key>
                  <Property Name="CourseID" Type="Int32" Nullable="false" />
                  <Property Name="Title" Type="String" Nullable="false" MaxLength="50" />
                </EntityType>
                <EntityType Name="Student">
                  <Key>
                    <PropertyRef Name="StudentID" />
                  </Key>
                  <Property Name="StudentID" Type="Int32" Nullable="false" />
                  <Property Name="LastName" Type="String" Nullable="false" MaxLength="50" />
                </EntityType>
                <EntityContainer Name="SchoolContext">
                  <EntitySet Name="Courses" EntityType="SchoolModel.Cours" />
                  <EntitySet Name="Students" EntityType="SchoolModel.Student" />
                </EntityContainer>
              </Schema>
            </edmx:ConceptualModels>
            <edmx:Mappings>
              <Mapping Space="C-S" xmlns="http://schemas.microsoft.com/ado/2009/11/mapping/cs">
                <EntityContainerMapping StorageEntityContainer="SchoolModelStoreContainer" CdmEntityContainer="SchoolContext">
                </EntityContainerMapping>
              </Mapping>
            </edmx:Mappings>
          </edmx:Runtime>
        </edmx:Edmx>
        """;

    private const string SampleEdmx = """
        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="3.0" xmlns:edmx="http://schemas.microsoft.com/ado/2009/11/edmx">
          <edmx:Runtime>
            <edmx:ConceptualModels>
              <Schema Namespace="StoreModel" xmlns="http://schemas.microsoft.com/ado/2009/11/edm" xmlns:annotation="http://schemas.microsoft.com/ado/2009/02/edm/annotation">
                <EntityType Name="Customer">
                  <Key>
                    <PropertyRef Name="Id" />
                  </Key>
                  <Property Name="Id" Type="Int32" Nullable="false" annotation:StoreGeneratedPattern="Identity" />
                  <Property Name="Name" Type="String" Nullable="false" MaxLength="128" />
                  <NavigationProperty Name="Orders" Relationship="StoreModel.FK_Order_Customer" FromRole="Customer" ToRole="Orders" />
                </EntityType>
                <EntityType Name="Order">
                  <Key>
                    <PropertyRef Name="Id" />
                  </Key>
                  <Property Name="Id" Type="Int32" Nullable="false" annotation:StoreGeneratedPattern="Identity" />
                  <Property Name="CustomerId" Type="Int32" Nullable="false" />
                  <Property Name="OrderDate" Type="DateTime" Nullable="false" />
                  <NavigationProperty Name="Customer" Relationship="StoreModel.FK_Order_Customer" FromRole="Orders" ToRole="Customer" />
                </EntityType>
                <EntityType Name="Product">
                  <Key>
                    <PropertyRef Name="Id" />
                  </Key>
                  <Property Name="Id" Type="Int32" Nullable="false" annotation:StoreGeneratedPattern="Identity" />
                  <Property Name="Title" Type="String" Nullable="false" MaxLength="256" />
                </EntityType>
                <Association Name="FK_Order_Customer">
                  <End Role="Customer" Type="StoreModel.Customer" Multiplicity="1">
                    <OnDelete Action="Cascade" />
                  </End>
                  <End Role="Orders" Type="StoreModel.Order" Multiplicity="*" />
                  <ReferentialConstraint>
                    <Principal Role="Customer">
                      <PropertyRef Name="Id" />
                    </Principal>
                    <Dependent Role="Orders">
                      <PropertyRef Name="CustomerId" />
                    </Dependent>
                  </ReferentialConstraint>
                </Association>
                <EntityContainer Name="StoreContext">
                  <EntitySet Name="Customers" EntityType="StoreModel.Customer" />
                  <EntitySet Name="Orders" EntityType="StoreModel.Order" />
                  <EntitySet Name="Products" EntityType="StoreModel.Product" />
                </EntityContainer>
              </Schema>
            </edmx:ConceptualModels>
            <edmx:Mappings>
              <Mapping Space="C-S" xmlns="http://schemas.microsoft.com/ado/2009/11/mapping/cs">
                <EntityContainerMapping StorageEntityContainer="StoreModelStoreContainer" CdmEntityContainer="StoreContext">
                  <EntitySetMapping Name="Customers">
                    <EntityTypeMapping TypeName="IsTypeOf(StoreModel.Customer)">
                      <MappingFragment StoreEntitySet="tblCustomers">
                        <ScalarProperty Name="Id" ColumnName="CustomerId" />
                        <ScalarProperty Name="Name" ColumnName="CustomerName" />
                      </MappingFragment>
                    </EntityTypeMapping>
                  </EntitySetMapping>
                  <EntitySetMapping Name="Orders">
                    <EntityTypeMapping TypeName="IsTypeOf(StoreModel.Order)">
                      <MappingFragment StoreEntitySet="tblOrders">
                        <ScalarProperty Name="Id" ColumnName="OrderId" />
                        <ScalarProperty Name="CustomerId" ColumnName="CustomerId" />
                        <ScalarProperty Name="OrderDate" ColumnName="OrderDate" />
                      </MappingFragment>
                    </EntityTypeMapping>
                  </EntitySetMapping>
                  <EntitySetMapping Name="Products">
                    <EntityTypeMapping TypeName="IsTypeOf(StoreModel.Product)">
                      <MappingFragment StoreEntitySet="tblProducts">
                        <ScalarProperty Name="Id" ColumnName="ProductId" />
                        <ScalarProperty Name="Title" ColumnName="Title" />
                      </MappingFragment>
                    </EntityTypeMapping>
                  </EntitySetMapping>
                </EntityContainerMapping>
              </Mapping>
            </edmx:Mappings>
          </edmx:Runtime>
        </edmx:Edmx>
        """;
}
