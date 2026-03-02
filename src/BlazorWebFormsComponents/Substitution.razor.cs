using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents
{
	public partial class Substitution : BaseWebFormsComponent
	{
		/// <summary>
		/// Gets or sets the name of the callback method. Preserved for migration reference only.
		/// </summary>
		[Parameter]
		public string MethodName { get; set; }

		/// <summary>
		/// A callback that receives the current HttpContext and returns markup to render.
		/// This is the Blazor equivalent of the Web Forms static callback method.
		/// </summary>
		[Parameter]
		public Func<HttpContext, string> SubstitutionCallback { get; set; }
	}
}
