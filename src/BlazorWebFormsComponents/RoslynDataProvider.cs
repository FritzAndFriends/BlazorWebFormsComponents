using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorWebFormsComponents
{
  public class RoslynDataProvider
  {


	  //private Project GetWebApiProject()
	  //{
		 // var work = MSBuildWorkspace.Create();
		 // var solution = work.OpenSolutionAsync(PathToSolution).Result;
		 // var project = solution.Projects.FirstOrDefault(p =>
			//  p.Name.ToUpper().EndsWith("WEBAPI"));
		 // if (project == null)
			//  throw new ApplicationException(
			//	  "WebApi project not found in solution " + PathToSolution);
		 // return project;
	  //}

	  //public IEnumerable<ClassDeclarationSyntax> FindControllers(Project project)
	  //{
		 // compilation = project.GetCompilationAsync().Result;
		 // var targetType = compilation.GetTypeByMetadataName(
			//  "System.Web.Http.ApiController");
		 // foreach (var document in project.Documents)
		 // {
			//  var tree = document.GetSyntaxTreeAsync().Result;
			//  var semanticModel = compilation.GetSemanticModel(tree);
			//  foreach (var type in tree.GetRoot().DescendantNodes().
			//	  OfType<ClassDeclarationSyntax>()
			//	  .Where(type => GetBaseClasses(semanticModel, type).Contains(targetType)))
			//  {
			//	  yield return type;
			//  }
		 // }
	  //}

	  public static IEnumerable<INamedTypeSymbol> GetBaseClasses
		  (SemanticModel model, BaseTypeDeclarationSyntax type)
	  {
		  var classSymbol = model.GetDeclaredSymbol(type);
		  var returnValue = new List<INamedTypeSymbol>();
		  while (classSymbol.BaseType != null)
		  {
			  returnValue.Add(classSymbol.BaseType);
			  if (classSymbol.Interfaces != null)
				  returnValue.AddRange(classSymbol.Interfaces);
			  classSymbol = classSymbol.BaseType;
		  }
		  return returnValue;
	  }

	}
}
