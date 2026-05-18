using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace BlazorWebFormsComponents;

/// <summary>
/// Event arguments for <see cref="WebFormsForm"/> submissions.
/// Contains the form field data captured via JS interop in interactive mode.
/// </summary>
public sealed class FormSubmitEventArgs : EventArgs
{
	/// <summary>
	/// Gets the submitted form field data as a dictionary of field names to values.
	/// Multi-value fields (e.g., checkboxes) have multiple values in the
	/// <see cref="StringValues"/> entry.
	/// </summary>
	public Dictionary<string, StringValues> FormData { get; }

	/// <summary>
	/// Initializes a new instance of <see cref="FormSubmitEventArgs"/>.
	/// </summary>
	/// <param name="formData">The captured form field data.</param>
	public FormSubmitEventArgs(Dictionary<string, StringValues> formData)
	{
		FormData = formData ?? throw new ArgumentNullException(nameof(formData));
	}
}
