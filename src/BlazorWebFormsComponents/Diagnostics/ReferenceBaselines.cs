using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorWebFormsComponents.Diagnostics
{
	/// <summary>
	/// Loads and provides access to reference baseline data (expected property/event counts)
	/// from the dev-docs/reference-baselines.json file.
	/// </summary>
	public class ReferenceBaselines
	{
		private readonly Dictionary<string, ComponentBaseline> _components = new Dictionary<string, ComponentBaseline>(StringComparer.OrdinalIgnoreCase);

		public string Version { get; private set; }
		public string Source { get; private set; }

		/// <summary>
		/// Tries to get the baseline for a given component name.
		/// Returns null if no baseline exists.
		/// </summary>
		public ComponentBaseline GetBaseline(string componentName)
		{
			_components.TryGetValue(componentName, out var baseline);
			return baseline;
		}

		/// <summary>
		/// Loads baselines from a JSON file. Returns an empty baselines instance if the file doesn't exist.
		/// </summary>
		public static ReferenceBaselines LoadFromFile(string filePath)
		{
			var baselines = new ReferenceBaselines();

			if (!File.Exists(filePath))
			{
				return baselines;
			}

			try
			{
				var json = File.ReadAllText(filePath);
				var doc = JsonSerializer.Deserialize<BaselinesDocument>(json, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});

				if (doc != null)
				{
					baselines.Version = doc.Version;
					baselines.Source = doc.Source;

					if (doc.Components != null)
					{
						foreach (var kvp in doc.Components)
						{
							baselines._components[kvp.Key] = kvp.Value;
						}
					}
				}
			}
			catch (JsonException)
			{
				// If the JSON is malformed, return empty baselines rather than crashing
			}

			return baselines;
		}

		private class BaselinesDocument
		{
			[JsonPropertyName("version")]
			public string Version { get; set; }

			[JsonPropertyName("source")]
			public string Source { get; set; }

			[JsonPropertyName("generated")]
			public string Generated { get; set; }

			[JsonPropertyName("components")]
			public Dictionary<string, ComponentBaseline> Components { get; set; }
		}
	}

	/// <summary>
	/// Reference baseline for a single Web Forms control, containing expected property and event counts.
	/// </summary>
	public class ComponentBaseline
	{
		[JsonPropertyName("namespace")]
		public string Namespace { get; set; }

		[JsonPropertyName("expectedProperties")]
		public int ExpectedProperties { get; set; }

		[JsonPropertyName("expectedEvents")]
		public int ExpectedEvents { get; set; }

		[JsonPropertyName("propertyList")]
		public string[] PropertyList { get; set; }

		[JsonPropertyName("eventList")]
		public string[] EventList { get; set; }

		[JsonPropertyName("notes")]
		public string Notes { get; set; }
	}
}
