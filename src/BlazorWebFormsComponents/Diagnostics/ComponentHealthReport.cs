namespace BlazorWebFormsComponents.Diagnostics
{
	/// <summary>
	/// Health report for a single BWFC component, measuring how completely it reproduces
	/// the behavior of its original ASP.NET Web Forms control.
	/// </summary>
	public class ComponentHealthReport
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public int ImplementedProperties { get; set; }
		public int? ExpectedProperties { get; set; }
		public int ImplementedEvents { get; set; }
		public int? ExpectedEvents { get; set; }
		public bool HasTests { get; set; }
		public bool HasDocumentation { get; set; }
		public bool HasSamplePage { get; set; }
		public ImplementationStatus Status { get; set; }
		public double HealthScore { get; set; }

		/// <summary>
		/// Property parity as a ratio (0.0 to 1.0), or null if no baseline exists.
		/// </summary>
		public double? PropertyParity { get; set; }

		/// <summary>
		/// Event parity as a ratio (0.0 to 1.0), or null if no baseline exists.
		/// </summary>
		public double? EventParity { get; set; }

		/// <summary>
		/// List of implemented property names (component-specific layer only).
		/// </summary>
		public string[] ImplementedPropertyNames { get; set; } = System.Array.Empty<string>();

		/// <summary>
		/// List of implemented event names (component-specific layer only).
		/// </summary>
		public string[] ImplementedEventNames { get; set; } = System.Array.Empty<string>();
	}
}
