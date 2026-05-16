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
        // Verifies that after EDMX conversion, Model1.cs, Model1.Context.cs, and
        // Model1.Designer.cs are all added to excludedSourceFiles so SourceFileCopier
        // won't copy the EF6 T4 artifacts alongside the EF Core output (CS0101).
        var sourceRoot = CreateDirectory("batch-excl");
        var modelsDir = CreateDirectory("batch-excl", "Models");
        var sourceFile = CreateFile("batch-excl", Path.Combine("Models", "Model1.edmx"), SampleEdmx);
        var outputPath = CreateDirectory("batch-excl", "out");

        // Create the T4-generated companion files that should be excluded
        var t4Context = Path.Combine(modelsDir, "Model1.Context.cs");
        var t4Entities = Path.Combine(modelsDir, "Model1.cs");
        var t4Designer = Path.Combine(modelsDir, "Model1.Designer.cs");
        File.WriteAllText(t4Context, "// T4 context");
        File.WriteAllText(t4Entities, "// T4 entities");
        File.WriteAllText(t4Designer, "// T4 designer");

        var converter = new EdmxToEfCoreConverter();
        var report = new BlazorWebFormsComponents.Cli.Pipeline.MigrationReport();
        var excluded = await converter.ConvertAsync(sourceRoot, outputPath, "StoreProject", dryRun: false, report);

        Assert.Contains(t4Context, excluded, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(t4Entities, excluded, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(t4Designer, excluded, StringComparer.OrdinalIgnoreCase);
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
