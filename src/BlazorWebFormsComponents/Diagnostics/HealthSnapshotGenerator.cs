using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorWebFormsComponents.Diagnostics
{
	/// <summary>
	/// Generates and loads pre-computed health report snapshots.
	/// Used to provide accurate health data in environments (e.g., Docker containers)
	/// where the repository filesystem is not available.
	/// </summary>
	public static class HealthSnapshotGenerator
	{
		/// <summary>
		/// The default filename for the health snapshot JSON file.
		/// </summary>
		public const string SnapshotFileName = "health-snapshot.json";

		private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
		};

		/// <summary>
		/// Generates a health snapshot JSON file from the given service.
		/// </summary>
		/// <param name="service">The health service to snapshot.</param>
		/// <param name="outputPath">Full path to write the JSON file.</param>
		public static void GenerateSnapshot(ComponentHealthService service, string outputPath)
		{
			var reports = service.GetAllReports();
			var json = JsonSerializer.Serialize(reports, SerializerOptions);
			var dir = Path.GetDirectoryName(outputPath);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			File.WriteAllText(outputPath, json);
		}

		/// <summary>
		/// Loads pre-computed health reports from a snapshot JSON file.
		/// Returns null if the file does not exist or cannot be parsed.
		/// </summary>
		/// <param name="snapshotPath">Full path to the snapshot JSON file.</param>
		public static IReadOnlyList<ComponentHealthReport>? LoadSnapshot(string snapshotPath)
		{
			if (!File.Exists(snapshotPath))
				return null;

			try
			{
				var json = File.ReadAllText(snapshotPath);
				var reports = JsonSerializer.Deserialize<List<ComponentHealthReport>>(json, SerializerOptions);
				return reports;
			}
			catch (JsonException)
			{
				return null;
			}
			catch (IOException)
			{
				return null;
			}
		}
	}
}
