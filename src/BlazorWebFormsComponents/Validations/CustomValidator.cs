using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents.Validations
{
	public class CustomValidator : BaseValidator<string>
	{
		[Parameter] public bool ValidateEmptyText { get; set; }
		[Parameter] public Func<string, bool> ServerValidate { get; set; }

		/// <summary>
		/// Gets or sets the name of a client-side validation function.
		/// Migration stub — Blazor does not execute client scripts.
		/// </summary>
		[Parameter] public string ClientValidationFunction { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the associated input control passes validation.
		/// </summary>
		[Parameter] public bool IsValid { get; set; } = true;

		public override bool Validate(string value)
		{
			if (!ValidateEmptyText && string.IsNullOrWhiteSpace(value))
			{
				return true;
			}

			return ServerValidate(value);
		}
	}
}
