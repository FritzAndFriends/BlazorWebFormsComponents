namespace BlazorWebFormsComponents.Diagnostics
{
	/// <summary>
	/// Tracks the implementation status of a BWFC component relative to its Web Forms original.
	/// </summary>
	public enum ImplementationStatus
	{
		/// <summary>Component is fully implemented and functional.</summary>
		Complete,

		/// <summary>Component exists but is a stub (e.g., ScriptManager renders nothing).</summary>
		Stub,

		/// <summary>Component implementation has been deferred.</summary>
		Deferred,

		/// <summary>Component has not been started yet.</summary>
		NotStarted
	}
}
