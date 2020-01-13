using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Validations
{
	public class RegularExpressionValidator : BaseValidator<string>
	{
		[Parameter] public string ValidationExpression { get; set; }

		[Parameter] public int? MatchTimeout { get; set; }

		public override bool Validate(string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}

			if (MatchTimeout.HasValue)
			{
				// Not sure if MatchTimeout is in seconds
				// Also don't know the default RegexOptions
				return Regex.IsMatch(value, ValidationExpression, RegexOptions.None, TimeSpan.FromSeconds(MatchTimeout.Value));
			}
			else
			{
				return Regex.IsMatch(value, ValidationExpression, RegexOptions.None);
			}
		}
	}
}
