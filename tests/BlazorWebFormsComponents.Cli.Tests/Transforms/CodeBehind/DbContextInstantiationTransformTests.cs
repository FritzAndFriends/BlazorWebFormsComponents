using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class DbContextInstantiationTransformTests
{
    private readonly DbContextInstantiationTransform _sut = new();

    [Fact]
    public void SelfInjection_IsExcluded()
    {
        var input = @"public class ShoppingCartActions
{
    public static ShoppingCartActions GetCart()
    {
        var cart = new ShoppingCartActions();
        return cart;
    }
}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "Logic/ShoppingCartActions.cs",
            OutputFilePath = "Logic/ShoppingCartActions.cs",
            FileType = FileType.CodeFile,
            OriginalContent = string.Empty
        };

        var result = _sut.Apply(input, metadata);

        result.ShouldNotContain("private readonly ShoppingCartActions");
        result.ShouldNotContain("ShoppingCartActions shoppingCartActions");
    }

    [Fact]
    public void ExternalDependency_StillInjected()
    {
        var input = @"public class ShoppingCartActions
{
    public void DoWork()
    {
        var db = new ProductContext();
        db.Products.ToList();
    }
}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "Logic/ShoppingCartActions.cs",
            OutputFilePath = "Logic/ShoppingCartActions.cs",
            FileType = FileType.CodeFile,
            OriginalContent = string.Empty
        };

        var result = _sut.Apply(input, metadata);

        result.ShouldContain("ProductContext");
        result.ShouldContain("_productContext");
    }

    [Fact]
    public void EntitiesSuffix_IsInjected_ForBllClass()
    {
        // Reproduces the ContosoUniversity pattern:
        // BLL classes use new ContosoUniversityEntities() inline in each method.
        // Previously the "Entities" suffix was not in the regex, so no injection occurred.
        var input = @"namespace ContosoUniversity.BLL;

public class Students_Logic
{
    public void DeleteStudent(int id)
    {
        ContosoUniversityEntities contextObj = new ContosoUniversityEntities();
        contextObj.Students.Remove(contextObj.Students.First(s => s.StudentID == id));
        contextObj.SaveChanges();
    }

    public List<object> GetStudents(string name)
    {
        ContosoUniversityEntities contextObj = new ContosoUniversityEntities();
        return contextObj.Students.Select(s => (object)s).ToList();
    }
}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "BLL/Students_Logic.cs",
            OutputFilePath = "BLL/Students_Logic.cs",
            FileType = FileType.CodeFile,
            OriginalContent = string.Empty
        };

        var result = _sut.Apply(input, metadata);

        // A private readonly field + constructor injection should be emitted
        result.ShouldContain("private readonly ContosoUniversityEntities");
        // Inline new() calls should be replaced with the injected field reference
        result.ShouldNotContain("new ContosoUniversityEntities()");
        // The injected field name should follow the _camelCase convention
        result.ShouldContain("_contosoUniversityEntities");
    }

    [Fact]
    public void EntitiesSuffix_InlineExpression_IsInjected()
    {
        // Covers pattern like: from x in new ContosoUniversityEntities().Courses
        var input = @"namespace ContosoUniversity.BLL;

public class Courses_Logic
{
    public List<Cours> GetCourses(string department)
    {
        var courses = (from cours in new ContosoUniversityEntities().Courses
                       where cours.Department.DepartmentName == department
                       select cours).ToList();
        return courses;
    }
}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "BLL/Courses_Logic.cs",
            OutputFilePath = "BLL/Courses_Logic.cs",
            FileType = FileType.CodeFile,
            OriginalContent = string.Empty
        };

        var result = _sut.Apply(input, metadata);

        result.ShouldNotContain("new ContosoUniversityEntities()");
        result.ShouldContain("_contosoUniversityEntities");
    }

    [Fact]
    public void DataContextSuffix_IsInjected()
    {
        // Covers LINQ-to-SQL DataContext subclass pattern
        var input = @"public class ProductService
{
    public void Save(Product p)
    {
        var db = new NorthwindDataContext();
        db.Products.InsertOnSubmit(p);
        db.SubmitChanges();
    }
}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "Services/ProductService.cs",
            OutputFilePath = "Services/ProductService.cs",
            FileType = FileType.CodeFile,
            OriginalContent = string.Empty
        };

        var result = _sut.Apply(input, metadata);

        result.ShouldNotContain("new NorthwindDataContext()");
        result.ShouldContain("_northwindDataContext");
    }
}
