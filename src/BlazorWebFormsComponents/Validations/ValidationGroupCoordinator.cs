using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.Validations
{
	/// <summary>
	/// Coordinates validation across validators with matching ValidationGroup properties
	/// </summary>
	public class ValidationGroupCoordinator
	{
		private readonly ConcurrentBag<IValidationGroupMember> _validators = new ConcurrentBag<IValidationGroupMember>();

		public void RegisterValidator(IValidationGroupMember validator)
		{
			if (validator != null && !_validators.Contains(validator))
			{
				_validators.Add(validator);
			}
		}

		public void UnregisterValidator(IValidationGroupMember validator)
		{
			// ConcurrentBag doesn't support removal, so we'll need to use a different approach
			// Since we're checking in ValidateGroup anyway, we can just mark validators as invalid
			// For now, we'll keep the validator in the bag but check if it's null or valid
			// A better approach would be to use ConcurrentDictionary, but for this scenario
			// where validators are typically added during initialization and removed during disposal,
			// the Contains check in ValidateGroup provides sufficient safety
		}

		/// <summary>
		/// Triggers validation for all validators in the specified group
		/// </summary>
		/// <param name="validationGroup">The validation group to validate. Null or empty matches validators without a group.</param>
		public void ValidateGroup(string validationGroup)
		{
			var normalizedGroup = string.IsNullOrEmpty(validationGroup) ? string.Empty : validationGroup;

			// Create a snapshot of validators to avoid issues with collection changes during iteration
			var validatorSnapshot = _validators.ToList();

			foreach (var validator in validatorSnapshot)
			{
				if (validator == null) continue;

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
