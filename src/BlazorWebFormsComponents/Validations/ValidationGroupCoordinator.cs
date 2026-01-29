using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.Validations
{
	/// <summary>
	/// Coordinates validation across validators with matching ValidationGroup properties
	/// </summary>
	public class ValidationGroupCoordinator
	{
		private readonly List<IValidationGroupMember> _validators = new List<IValidationGroupMember>();

		public void RegisterValidator(IValidationGroupMember validator)
		{
			if (!_validators.Contains(validator))
			{
				_validators.Add(validator);
			}
		}

		public void UnregisterValidator(IValidationGroupMember validator)
		{
			_validators.Remove(validator);
		}

		/// <summary>
		/// Triggers validation for all validators in the specified group
		/// </summary>
		/// <param name="validationGroup">The validation group to validate. Null or empty matches validators without a group.</param>
		public void ValidateGroup(string validationGroup)
		{
			var normalizedGroup = string.IsNullOrEmpty(validationGroup) ? string.Empty : validationGroup;

			foreach (var validator in _validators)
			{
				var validatorGroup = string.IsNullOrEmpty(validator.ValidationGroup) ? string.Empty : validator.ValidationGroup;
				
				if (validatorGroup == normalizedGroup)
				{
					validator.PerformValidation();
				}
			}
		}
	}

	/// <summary>
	/// Interface for components that participate in validation groups
	/// </summary>
	public interface IValidationGroupMember
	{
		string ValidationGroup { get; }
		void PerformValidation();
	}
}
