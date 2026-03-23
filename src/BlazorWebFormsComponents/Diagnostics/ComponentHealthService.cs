using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BlazorWebFormsComponents.DataBinding;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Diagnostics
{
	/// <summary>
	/// Analyzes BWFC components via reflection and file scanning to produce health reports
	/// measuring how completely each Blazor component reproduces its Web Forms original.
	/// When the repository filesystem is not available (e.g., in Docker), the service can
	/// serve pre-computed reports from a snapshot JSON file.
	/// </summary>
	public class ComponentHealthService
	{
		private readonly ReferenceBaselines _baselines;
		private readonly string _solutionRoot;
		private readonly IReadOnlyList<ComponentHealthReport> _snapshotReports;

		private static readonly HashSet<Type> StopTypes = new HashSet<Type>
		{
			typeof(BaseWebFormsComponent),
			typeof(BaseStyledComponent),
			typeof(BaseDataBoundComponent)
		};

		/// <summary>
		/// Maps actual class names to their tracked component names when
		/// the two differ (e.g. AspNetValidationSummary → ValidationSummary).
		/// </summary>
		private static readonly Dictionary<string, string> TypeAliases =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["AspNetValidationSummary"] = "ValidationSummary"
		};

		/// <summary>
		/// Returns true when this instance is serving pre-computed snapshot data
		/// rather than live reflection/file-scanning results.
		/// </summary>
		public bool IsFromSnapshot => _snapshotReports != null;

		// Hardcoded fallback per PRD §3.3 — used when dev-docs/tracked-components.json doesn't exist
		private static readonly Dictionary<string, TrackedComponent> DefaultTrackedComponents = new Dictionary<string, TrackedComponent>(StringComparer.OrdinalIgnoreCase)
		{
			// Editor Controls
			["AdRotator"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Button"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["BulletedList"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Calendar"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["CheckBox"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["CheckBoxList"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["DropDownList"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["FileUpload"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["HiddenField"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["HyperLink"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Image"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["ImageButton"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["ImageMap"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Label"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["LinkButton"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["ListBox"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Literal"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Localize"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["MultiView"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Panel"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["PlaceHolder"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["RadioButton"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["RadioButtonList"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Table"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["TextBox"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["View"] = new TrackedComponent("Editor", ImplementationStatus.Complete),
			["Substitution"] = new TrackedComponent("Editor", ImplementationStatus.Deferred),
			["Xml"] = new TrackedComponent("Editor", ImplementationStatus.Deferred),

			// Data Controls
			["Chart"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["DataGrid"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["DataList"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["DataPager"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["DetailsView"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["FormView"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["GridView"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["ListView"] = new TrackedComponent("Data", ImplementationStatus.Complete),
			["Repeater"] = new TrackedComponent("Data", ImplementationStatus.Complete),

			// Validation Controls
			["CompareValidator"] = new TrackedComponent("Validation", ImplementationStatus.Complete),
			["CustomValidator"] = new TrackedComponent("Validation", ImplementationStatus.Complete),
			["RangeValidator"] = new TrackedComponent("Validation", ImplementationStatus.Complete),
			["RegularExpressionValidator"] = new TrackedComponent("Validation", ImplementationStatus.Complete),
			["RequiredFieldValidator"] = new TrackedComponent("Validation", ImplementationStatus.Complete),
			["ValidationSummary"] = new TrackedComponent("Validation", ImplementationStatus.Complete),

			// Navigation Controls
			["Menu"] = new TrackedComponent("Navigation", ImplementationStatus.Complete),
			["SiteMapPath"] = new TrackedComponent("Navigation", ImplementationStatus.Complete),
			["TreeView"] = new TrackedComponent("Navigation", ImplementationStatus.Complete),

			// Login Controls
			["ChangePassword"] = new TrackedComponent("Login", ImplementationStatus.Complete),
			["CreateUserWizard"] = new TrackedComponent("Login", ImplementationStatus.Complete),
			["Login"] = new TrackedComponent("Login", ImplementationStatus.Complete),
			["LoginName"] = new TrackedComponent("Login", ImplementationStatus.Complete),
			["LoginStatus"] = new TrackedComponent("Login", ImplementationStatus.Complete),
			["LoginView"] = new TrackedComponent("Login", ImplementationStatus.Complete),
			["PasswordRecovery"] = new TrackedComponent("Login", ImplementationStatus.Complete),

			// Infrastructure
			["Content"] = new TrackedComponent("Infrastructure", ImplementationStatus.Complete),
			["ContentPlaceHolder"] = new TrackedComponent("Infrastructure", ImplementationStatus.Complete),
			["MasterPage"] = new TrackedComponent("Infrastructure", ImplementationStatus.Complete),
			["NamingContainer"] = new TrackedComponent("Infrastructure", ImplementationStatus.Complete),
			["ScriptManager"] = new TrackedComponent("Infrastructure", ImplementationStatus.Stub),
			["UpdatePanel"] = new TrackedComponent("Infrastructure", ImplementationStatus.Complete),
		};

		private static readonly HashSet<string> ExcludedPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"AdditionalAttributes",
			"ChildContent",
			"ChildComponents"
		};

		private Dictionary<string, TrackedComponent> _trackedComponents;
		private Dictionary<string, Type> _discoveredTypes;

		/// <summary>
		/// Creates a new ComponentHealthService that performs live reflection and file scanning.
		/// </summary>
		/// <param name="baselines">Reference baselines loaded from JSON.</param>
		/// <param name="solutionRoot">Path to the repository root (for file scanning).</param>
		public ComponentHealthService(ReferenceBaselines baselines, string solutionRoot)
		{
			_baselines = baselines ?? new ReferenceBaselines();
			_solutionRoot = solutionRoot ?? "";
			_snapshotReports = null;
			_trackedComponents = LoadTrackedComponents();
			_discoveredTypes = DiscoverComponentTypes();
		}

		/// <summary>
		/// Creates a ComponentHealthService that serves pre-computed snapshot data.
		/// Used in environments where the repository filesystem is not available.
		/// </summary>
		/// <param name="snapshotReports">Pre-computed health reports loaded from a snapshot file.</param>
		internal ComponentHealthService(IReadOnlyList<ComponentHealthReport> snapshotReports)
		{
			_snapshotReports = snapshotReports ?? throw new ArgumentNullException(nameof(snapshotReports));
			_baselines = new ReferenceBaselines();
			_solutionRoot = "";
			_trackedComponents = new Dictionary<string, TrackedComponent>(StringComparer.OrdinalIgnoreCase);
			_discoveredTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets health reports for all tracked components.
		/// When running from a snapshot, returns the pre-computed reports.
		/// </summary>
		public IReadOnlyList<ComponentHealthReport> GetAllReports()
		{
			if (_snapshotReports != null)
				return _snapshotReports;

			var reports = new List<ComponentHealthReport>();
			foreach (var kvp in _trackedComponents.OrderBy(k => k.Value.Category).ThenBy(k => k.Key))
			{
				reports.Add(BuildReport(kvp.Key, kvp.Value));
			}
			return reports;
		}

		/// <summary>
		/// Gets a health report for a single component by name.
		/// Returns null if the component is not tracked.
		/// </summary>
		public ComponentHealthReport GetReport(string componentName)
		{
			if (_snapshotReports != null)
				return _snapshotReports.FirstOrDefault(r => r.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));

			if (!_trackedComponents.TryGetValue(componentName, out var tracked))
				return null;
			return BuildReport(componentName, tracked);
		}

		private ComponentHealthReport BuildReport(string componentName, TrackedComponent tracked)
		{
			_discoveredTypes.TryGetValue(componentName, out var componentType);

			var (propCount, eventCount, propNames, eventNames) = componentType != null
				? CountPropertiesAndEvents(componentType)
				: (0, 0, Array.Empty<string>(), Array.Empty<string>());

			var baseline = _baselines.GetBaseline(componentName);
			var hasBaseline = baseline != null;

			var expectedProps = hasBaseline ? baseline.ExpectedProperties : (int?)null;
			var expectedEvents = hasBaseline ? baseline.ExpectedEvents : (int?)null;

			var propParity = ComputeParity(propCount, expectedProps);
			var eventParity = ComputeParity(eventCount, expectedEvents);

			var hasTests = DetectTests(componentName);
			var hasDocs = DetectDocumentation(componentName);
			var hasSample = DetectSamplePage(componentName);

			var healthScore = ComputeHealthScore(
				propParity, eventParity, hasTests, hasDocs, hasSample, tracked.Status);

			return new ComponentHealthReport
			{
				Name = componentName,
				Category = tracked.Category,
				ImplementedProperties = propCount,
				ExpectedProperties = expectedProps,
				ImplementedEvents = eventCount,
				ExpectedEvents = expectedEvents,
				HasTests = hasTests,
				HasDocumentation = hasDocs,
				HasSamplePage = hasSample,
				Status = tracked.Status,
				HealthScore = healthScore,
				PropertyParity = propParity,
				EventParity = eventParity,
				ImplementedPropertyNames = propNames,
				ImplementedEventNames = eventNames
			};
		}

		#region Property & Event Counting (§5.4)

		/// <summary>
		/// Walks the inheritance chain from leaf type upward, stopping at base stop-types.
		/// Counts [Parameter] properties using DeclaredOnly at each level.
		/// </summary>
		internal static (int properties, int events, string[] propertyNames, string[] eventNames) CountPropertiesAndEvents(Type componentType)
		{
			var properties = new List<string>();
			var events = new List<string>();

			var currentType = componentType;
			while (currentType != null && !IsStopType(currentType))
			{
				var declaredProps = currentType.GetProperties(
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				foreach (var prop in declaredProps)
				{
					if (!HasParameterAttribute(prop))
						continue;

					// [Obsolete] properties are still counted — they represent
					// migration-compatible implementations (Pattern B+).

					if (HasCascadingParameterAttribute(prop))
						continue;

					if (ExcludedPropertyNames.Contains(prop.Name))
						continue;

					if (IsRenderFragmentType(prop.PropertyType))
						continue;

					if (IsEventCallbackType(prop.PropertyType))
					{
						events.Add(prop.Name);
					}
					else
					{
						properties.Add(prop.Name);
					}
				}

				currentType = currentType.BaseType;
			}

			return (properties.Count, events.Count, properties.ToArray(), events.ToArray());
		}

		private static bool IsStopType(Type type)
		{
			var checkType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
			return StopTypes.Contains(checkType);
		}

		private static bool HasParameterAttribute(PropertyInfo prop)
		{
			return prop.GetCustomAttribute<ParameterAttribute>() != null;
		}

		private static bool HasObsoleteAttribute(PropertyInfo prop)
		{
			return prop.GetCustomAttribute<ObsoleteAttribute>() != null;
		}

		private static bool HasCascadingParameterAttribute(PropertyInfo prop)
		{
			return prop.GetCustomAttribute<CascadingParameterAttribute>() != null;
		}

		private static bool IsRenderFragmentType(Type type)
		{
			if (type == typeof(RenderFragment))
				return true;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RenderFragment<>))
				return true;
			return false;
		}

		private static bool IsEventCallbackType(Type type)
		{
			if (type == typeof(EventCallback))
				return true;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EventCallback<>))
				return true;
			return false;
		}

		#endregion

		#region Component Discovery (§5.1-5.2)

		private Dictionary<string, Type> DiscoverComponentTypes()
		{
			var assembly = typeof(BaseWebFormsComponent).Assembly;
			var result = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

			var allTypes = assembly.GetExportedTypes()
				.Where(t => t.IsClass && !t.IsAbstract)
				.Where(t => typeof(ComponentBase).IsAssignableFrom(t));

			foreach (var type in allTypes)
			{
				var cleanName = StripGenericArity(type.Name);

				// Resolve aliases for components whose class name differs from the tracked name
				if (TypeAliases.TryGetValue(cleanName, out var alias))
					cleanName = alias;

				if (_trackedComponents.ContainsKey(cleanName) && !result.ContainsKey(cleanName))
				{
					result[cleanName] = type;
				}
			}

			return result;
		}

		internal static string StripGenericArity(string typeName)
		{
			var backtickIndex = typeName.IndexOf('`');
			return backtickIndex >= 0 ? typeName.Substring(0, backtickIndex) : typeName;
		}

		#endregion

		#region Tracked Components Loading

		private Dictionary<string, TrackedComponent> LoadTrackedComponents()
		{
			if (!string.IsNullOrEmpty(_solutionRoot))
			{
				var jsonPath = Path.Combine(_solutionRoot, "dev-docs", "tracked-components.json");
				if (File.Exists(jsonPath))
				{
					try
					{
						var json = File.ReadAllText(jsonPath);
						var doc = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, TrackedComponentJson>>(
							json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

						if (doc != null && doc.Count > 0)
						{
							var result = new Dictionary<string, TrackedComponent>(StringComparer.OrdinalIgnoreCase);
							foreach (var kvp in doc)
							{
								var status = ParseStatus(kvp.Value.Status);
								result[kvp.Key] = new TrackedComponent(kvp.Value.Category ?? "Unknown", status);
							}
							return result;
						}
					}
					catch (System.Text.Json.JsonException)
					{
						// Fall through to default list
					}
				}
			}

			return new Dictionary<string, TrackedComponent>(DefaultTrackedComponents, StringComparer.OrdinalIgnoreCase);
		}

		private static ImplementationStatus ParseStatus(string status)
		{
			if (string.IsNullOrEmpty(status))
				return ImplementationStatus.Complete;

			if (status.Equals("Complete", StringComparison.OrdinalIgnoreCase))
				return ImplementationStatus.Complete;
			if (status.Equals("Stub", StringComparison.OrdinalIgnoreCase))
				return ImplementationStatus.Stub;
			if (status.Equals("Deferred", StringComparison.OrdinalIgnoreCase))
				return ImplementationStatus.Deferred;
			if (status.Equals("NotStarted", StringComparison.OrdinalIgnoreCase))
				return ImplementationStatus.NotStarted;

			return ImplementationStatus.Complete;
		}

		#endregion

		#region File Detection (§7.4)

		/// <summary>
		/// Scans test project for files containing the component name.
		/// </summary>
		private bool DetectTests(string componentName)
		{
			var testDir = Path.Combine(_solutionRoot, "src", "BlazorWebFormsComponents.Test");
			if (!Directory.Exists(testDir))
				return false;

			// Check for a directory matching the component name
			try
			{
				var matchingDirs = Directory.GetDirectories(testDir, componentName, SearchOption.AllDirectories);
				if (matchingDirs.Length > 0)
					return true;

				// Check for .razor or .cs files containing the component name
				var razorFiles = Directory.GetFiles(testDir, $"*{componentName}*", SearchOption.AllDirectories)
					.Where(f => f.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)
							 || f.EndsWith(".cs", StringComparison.OrdinalIgnoreCase));
				return razorFiles.Any();
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Scans docs/ for .md files matching the component name.
		/// </summary>
		private bool DetectDocumentation(string componentName)
		{
			var docsDir = Path.Combine(_solutionRoot, "docs");
			if (!Directory.Exists(docsDir))
				return false;

			try
			{
				// Check for ComponentName.md anywhere under docs/
				var matchingFiles = Directory.GetFiles(docsDir, $"{componentName}.md", SearchOption.AllDirectories);
				return matchingFiles.Length > 0;
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Checks if the component appears in the sample app's ComponentCatalog.
		/// Scans the ComponentCatalog.cs file for the component name.
		/// </summary>
		private bool DetectSamplePage(string componentName)
		{
			var catalogPath = Path.Combine(_solutionRoot, "samples", "AfterBlazorServerSide", "ComponentCatalog.cs");
			if (!File.Exists(catalogPath))
				return false;

			try
			{
				var content = File.ReadAllText(catalogPath);
				// Look for the component name as a catalog entry, e.g. new("Button",
				return content.Contains($"\"{componentName}\"", StringComparison.OrdinalIgnoreCase);
			}
			catch (IOException)
			{
				return false;
			}
		}

		#endregion

		#region Score Computation (§4.1)

		/// <summary>
		/// Computes parity ratio. Returns null when no baseline exists.
		/// Handles 0/0 as 1.0 (complete by definition per §4.3).
		/// </summary>
		private static double? ComputeParity(int implemented, int? expected)
		{
			if (!expected.HasValue)
				return null;

			if (expected.Value == 0)
				return 1.0; // 0/0 = complete by definition

			return Math.Min((double)implemented / expected.Value, 1.0);
		}

		/// <summary>
		/// Computes weighted health score per §4.1.
		/// Missing baselines (null parity) are excluded and weights re-distributed (§4.4).
		/// </summary>
		private static double ComputeHealthScore(
			double? propParity,
			double? eventParity,
			bool hasTests,
			bool hasDocs,
			bool hasSample,
			ImplementationStatus status)
		{
			// Define dimensions with their weights
			var dimensions = new List<(double weight, double score)>();

			// Property Parity: 30%
			if (propParity.HasValue)
				dimensions.Add((0.30, propParity.Value));

			// Event Parity: 15%
			if (eventParity.HasValue)
				dimensions.Add((0.15, eventParity.Value));

			// Has bUnit Tests: 20% (binary)
			dimensions.Add((0.20, hasTests ? 1.0 : 0.0));

			// Has Documentation: 15% (binary)
			dimensions.Add((0.15, hasDocs ? 1.0 : 0.0));

			// Has Sample Page: 10% (binary)
			dimensions.Add((0.10, hasSample ? 1.0 : 0.0));

			// Implementation Status: 10%
			double statusScore;
			switch (status)
			{
				case ImplementationStatus.Complete:
					statusScore = 1.0;
					break;
				case ImplementationStatus.Stub:
					statusScore = 0.5;
					break;
				default:
					statusScore = 0.0;
					break;
			}
			dimensions.Add((0.10, statusScore));

			// Re-weight proportionally if baselines are missing
			double totalWeight = 0;
			double weightedSum = 0;
			foreach (var (weight, score) in dimensions)
			{
				totalWeight += weight;
				weightedSum += weight * score;
			}

			if (totalWeight <= 0)
				return 0;

			var healthScore = (weightedSum / totalWeight) * 100.0;
			return Math.Min(Math.Round(healthScore, 1), 100.0);
		}

		#endregion

		#region Supporting Types

		private class TrackedComponent
		{
			public string Category { get; }
			public ImplementationStatus Status { get; }

			public TrackedComponent(string category, ImplementationStatus status)
			{
				Category = category;
				Status = status;
			}
		}

		private class TrackedComponentJson
		{
			public string Category { get; set; }
			public string Status { get; set; }
			public string WebFormsType { get; set; }
		}

		#endregion
	}
}
