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
				return Regex.IsMatch(value, ValidationExpression, RegexOptions.None, TimeSpan.FromMilliseconds(MatchTimeout.Value));
			}
			else
			{
				return Regex.IsMatch(value, ValidationExpression, RegexOptions.None);
			}
		}
	}
}
