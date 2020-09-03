using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents.Validations
{
	public class CustomValidator : BaseValidator<string>
	{
		[Parameter] public bool ValidateEmptyText { get; set; }
		[Parameter] public Func<string, bool> ServerValidate { get; set; }

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
