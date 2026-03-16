using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

// Alias to avoid conflicts with test project subfolders (Button/, Label/, etc.)
using BWF = BlazorWebFormsComponents;

namespace BlazorWebFormsComponents.Test.Diagnostics;

/// <summary>
/// Acceptance tests for the Component Health Dashboard counting algorithm (PRD §5.4, §10).
/// These verify that reflection-based property/event counting produces correct results
/// for known BWFC components. The algorithm is implemented inline per PRD guidance —
/// when ComponentHealthService lands, these become its integration tests.
/// </summary>
public class ComponentHealthCountingTests
{
	// The BWFC assembly containing all components
	private static readonly Assembly BwfAssembly = typeof(BWF.BaseWebFormsComponent).Assembly;

	#region Stop-type definitions (PRD §2.2 / §5.4)

	private static readonly HashSet<Type> StopTypes = new()
	{
		typeof(BWF.BaseWebFormsComponent),
		typeof(BWF.BaseStyledComponent),
		typeof(BWF.DataBinding.BaseDataBoundComponent),
	};

	#endregion

	#region Counting algorithm (PRD §5.4 — inline implementation)

	private static (int properties, int events) CountComponentSpecific(Type componentType)
	{
		int props = 0, events = 0;
		var current = componentType;

		while (current != null && !IsStopType(current))
		{
			foreach (var prop in current.GetProperties(
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
			{
				var paramAttr = prop.GetCustomAttribute<ParameterAttribute>();
				if (paramAttr == null) continue;
				if (prop.GetCustomAttribute<ObsoleteAttribute>() != null) continue;
				if (prop.GetCustomAttribute<CascadingParameterAttribute>() != null) continue;

				var propType = prop.PropertyType;

				if (propType == typeof(RenderFragment) ||
					(propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(RenderFragment<>)))
					continue;

				if (prop.Name is "AdditionalAttributes" or "ChildContent" or "ChildComponents")
					continue;

				if (propType == typeof(EventCallback) ||
					(propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(EventCallback<>)))
				{
					events++;
				}
				else
				{
					props++;
				}
			}

			current = current.BaseType;
		}

		return (props, events);
	}

	private static bool IsStopType(Type t)
	{
		if (StopTypes.Contains(t)) return true;
		if (t.IsGenericType)
		{
			var genericDef = t.GetGenericTypeDefinition();
			if (genericDef.Name.StartsWith("DataBoundComponent"))
				return true;
		}
		return false;
	}

	/// <summary>
	/// Strips the generic arity suffix from reflection type names (PRD §2.6).
	/// e.g. "GridView`1" → "GridView"
	/// </summary>
	private static string CleanTypeName(Type t)
	{
		var name = t.Name;
		var idx = name.IndexOf('`');
		return idx >= 0 ? name[..idx] : name;
	}

	/// <summary>
	/// Finds a non-abstract component type by clean name from the BWFC assembly.
	/// </summary>
	private static Type FindComponentType(string cleanName)
	{
		return BwfAssembly.GetTypes()
			.First(t => CleanTypeName(t) == cleanName && !t.IsAbstract);
	}

	/// <summary>
	/// Collects the names of all component-specific parameters, categorized.
	/// Useful for diagnostic output when a count is unexpected.
	/// </summary>
	private static (List<string> propertyNames, List<string> eventNames, List<string> skippedNames)
		GetParameterDetails(Type componentType)
	{
		var propertyNames = new List<string>();
		var eventNames = new List<string>();
		var skippedNames = new List<string>();
		var current = componentType;

		while (current != null && !IsStopType(current))
		{
			foreach (var prop in current.GetProperties(
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
			{
				var paramAttr = prop.GetCustomAttribute<ParameterAttribute>();
				if (paramAttr == null) { skippedNames.Add($"{current.Name}.{prop.Name} (no [Parameter])"); continue; }
				if (prop.GetCustomAttribute<ObsoleteAttribute>() != null) { skippedNames.Add($"{current.Name}.{prop.Name} ([Obsolete])"); continue; }
				if (prop.GetCustomAttribute<CascadingParameterAttribute>() != null) { skippedNames.Add($"{current.Name}.{prop.Name} ([CascadingParameter])"); continue; }

				var propType = prop.PropertyType;
				if (propType == typeof(RenderFragment) ||
					(propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(RenderFragment<>)))
				{
					skippedNames.Add($"{current.Name}.{prop.Name} (RenderFragment)");
					continue;
				}
				if (prop.Name is "AdditionalAttributes" or "ChildContent" or "ChildComponents")
				{
					skippedNames.Add($"{current.Name}.{prop.Name} (infrastructure)");
					continue;
				}

				if (propType == typeof(EventCallback) ||
					(propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(EventCallback<>)))
				{
					eventNames.Add($"{current.Name}.{prop.Name}");
				}
				else
				{
					propertyNames.Add($"{current.Name}.{prop.Name}");
				}
			}
			current = current.BaseType;
		}

		return (propertyNames, eventNames, skippedNames);
	}

	#endregion

	#region AC-1: Button shows correct property & event counts (PRD §10.1)

	[Fact]
	public void Button_ShowsCorrectPropertyAndEventCounts()
	{
		// Button → ButtonBaseComponent → BaseStyledComponent (stop)
		// PRD §2.7 worked example: ~7 properties, 2 events
		var buttonType = typeof(BWF.Button);
		var (props, events) = CountComponentSpecific(buttonType);
		var (propNames, eventNames, _) = GetParameterDetails(buttonType);

		// Events: OnClick, OnCommand
		events.ShouldBe(2, $"Event list: {string.Join(", ", eventNames)}");

		// Properties should be in realistic range — not 40+ (inherited inflation)
		// and not 1 (DeclaredOnly-too-far). PRD says ~7; may be 7 or 8 depending
		// on whether ButtonBaseComponent.PostBackUrl counts (see findings).
		props.ShouldBeInRange(7, 8, $"Property list: {string.Join(", ", propNames)}");
	}

	#endregion

	#region AC-2: GridView shows correct counts (PRD §10.2)

	[Fact]
	public void GridView_ShowsCorrectPropertyEventCounts()
	{
		// GridView<T> → DataBoundComponent<T> (stop)
		// PRD §2.7: ~18 properties, ~10 events (not 30+ or 0)
		var gridViewType = FindComponentType("GridView");
		var (props, events) = CountComponentSpecific(gridViewType);
		var (propNames, eventNames, _) = GetParameterDetails(gridViewType);

		// Must not be inflated by RenderFragment templates (12+) or base class props
		props.ShouldBeGreaterThan(10,
			$"Too few GridView properties. Got: {string.Join(", ", propNames)}");
		props.ShouldBeLessThanOrEqualTo(25,
			$"GridView properties inflated. Got: {string.Join(", ", propNames)}");

		// Events: ~10 (sort, page, select, row operations)
		events.ShouldBeGreaterThanOrEqualTo(8,
			$"Too few GridView events. Got: {string.Join(", ", eventNames)}");
		events.ShouldBeLessThanOrEqualTo(12,
			$"Too many GridView events. Got: {string.Join(", ", eventNames)}");
	}

	#endregion

	#region AC-3: Repeater shows 0 properties, 0 events (PRD §10.3)

	[Fact]
	public void Repeater_Shows0Properties0Events()
	{
		// Repeater<T> → DataBoundComponent<T> (stop)
		// All Repeater [Parameter]s are RenderFragment templates → excluded
		var repeaterType = FindComponentType("Repeater");
		var (props, events) = CountComponentSpecific(repeaterType);
		var (propNames, eventNames, skipped) = GetParameterDetails(repeaterType);

		props.ShouldBe(0,
			$"Repeater should have 0 properties (all RenderFragment). " +
			$"Got properties: {string.Join(", ", propNames)}");
		events.ShouldBe(0,
			$"Repeater should have 0 events. Got: {string.Join(", ", eventNames)}");

		// Verify templates were actually found and skipped
		skipped.ShouldContain(s => s.Contains("RenderFragment"),
			"Should have found and skipped RenderFragment templates on Repeater");
	}

	#endregion

	#region AC-4: Generic type name stripped correctly (PRD §10.4 / §2.6)

	[Fact]
	public void GenericType_NameStrippedCorrectly()
	{
		// GridView<T> reflects as "GridView`1" — must strip backtick for lookup
		var gridViewType = BwfAssembly.GetTypes()
			.First(t => t.Name.StartsWith("GridView`") && !t.IsAbstract);

		Assert.Contains("`", gridViewType.Name); // Sanity: GridView should be generic

		var cleanName = CleanTypeName(gridViewType);
		cleanName.ShouldBe("GridView");
		Assert.DoesNotContain("`", cleanName);
	}

	[Theory]
	[InlineData("Repeater")]
	[InlineData("GridView")]
	[InlineData("ListView")]
	[InlineData("DataList")]
	public void GenericComponents_CanBeFoundByCleanName(string expectedName)
	{
		var found = BwfAssembly.GetTypes()
			.Where(t => CleanTypeName(t) == expectedName && !t.IsAbstract)
			.ToList();

		found.ShouldNotBeEmpty(
			$"Should find component '{expectedName}' after stripping generic arity suffix");
	}

	#endregion

	#region AC-5: Base class properties never inflate counts (PRD §10.5)

	[Theory]
	[InlineData("ID")]
	[InlineData("CssClass")]
	[InlineData("BackColor")]
	[InlineData("ForeColor")]
	[InlineData("Enabled")]
	[InlineData("Visible")]
	[InlineData("Font")]
	[InlineData("Width")]
	[InlineData("Height")]
	[InlineData("TabIndex")]
	[InlineData("AccessKey")]
	[InlineData("ToolTip")]
	[InlineData("BorderColor")]
	[InlineData("BorderStyle")]
	[InlineData("BorderWidth")]
	public void BaseClassProperties_NeverAppearInComponentCounts(string basePropertyName)
	{
		// Check multiple representative components
		var componentTypes = new[]
		{
			typeof(BWF.Button),
			typeof(BWF.Label),
			typeof(BWF.TextBox),
			typeof(BWF.Panel),
		};

		foreach (var componentType in componentTypes)
		{
			var (propNames, eventNames, _) = GetParameterDetails(componentType);
			var allCounted = propNames.Concat(eventNames).ToList();

			allCounted.ShouldNotContain(
				s => s.EndsWith($".{basePropertyName}"),
				$"Base property '{basePropertyName}' should not appear in " +
				$"{componentType.Name}'s counted parameters. Found in: " +
				$"{string.Join(", ", allCounted)}");
		}
	}

	#endregion

	#region AC-6: EventCallback not double-counted as property (PRD §10.6)

	[Fact]
	public void EventCallback_NotDoubleCountedAsProperty()
	{
		// Button has OnClick and OnCommand — these must be events ONLY
		var (propNames, eventNames, _) = GetParameterDetails(typeof(BWF.Button));

		eventNames.ShouldContain(s => s.Contains("OnClick"),
			"OnClick should appear in events");
		eventNames.ShouldContain(s => s.Contains("OnCommand"),
			"OnCommand should appear in events");

		propNames.ShouldNotContain(s => s.Contains("OnClick"),
			"OnClick must NOT appear in properties");
		propNames.ShouldNotContain(s => s.Contains("OnCommand"),
			"OnCommand must NOT appear in properties");
	}

	[Fact]
	public void EventCallback_CountsAreDisjoint_AcrossComponents()
	{
		// For any component, no parameter name should appear in BOTH lists
		var typesToCheck = BwfAssembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract
				&& typeof(BWF.BaseWebFormsComponent).IsAssignableFrom(t))
			.Take(20);

		foreach (var type in typesToCheck)
		{
			var (propNames, eventNames, _) = GetParameterDetails(type);
			var propShort = propNames.Select(n => n.Split('.').Last()).ToHashSet();
			var eventShort = eventNames.Select(n => n.Split('.').Last()).ToHashSet();
			var overlap = propShort.Intersect(eventShort).ToList();

			overlap.ShouldBeEmpty(
				$"Component {CleanTypeName(type)} has parameters in both property AND event lists: " +
				$"{string.Join(", ", overlap)}");
		}
	}

	#endregion

	#region AC-7: RenderFragment excluded from all counts (PRD §10.7)

	[Fact]
	public void RenderFragment_ExcludedFromAllCounts()
	{
		// GridView has 12+ RenderFragment params — none should appear in counts
		var gridViewType = FindComponentType("GridView");
		var (propNames, eventNames, skipped) = GetParameterDetails(gridViewType);
		var allCounted = propNames.Concat(eventNames).ToList();

		// Known RenderFragment params that must be excluded
		var renderFragmentParams = new[]
		{
			"RowStyleContent", "AlternatingRowStyleContent", "HeaderStyleContent",
			"FooterStyleContent", "EmptyDataRowStyleContent", "PagerStyleContent",
			"EditRowStyleContent", "SelectedRowStyleContent", "PagerSettingsContent",
			"EmptyDataTemplate", "Columns", "ChildContent"
		};

		foreach (var rf in renderFragmentParams)
		{
			allCounted.ShouldNotContain(
				s => s.EndsWith($".{rf}"),
				$"RenderFragment parameter '{rf}' should be excluded from GridView counts. " +
				$"All counted: {string.Join(", ", allCounted)}");
		}

		// Verify they were found in skipped list
		skipped.Count(s => s.Contains("RenderFragment")).ShouldBeGreaterThan(5,
			"Should have found and skipped multiple RenderFragment templates on GridView");
	}

	[Fact]
	public void RenderFragment_ExcludedFromRepeater()
	{
		var repeaterType = FindComponentType("Repeater");
		var (propNames, eventNames, skipped) = GetParameterDetails(repeaterType);

		// All Repeater params are templates — ItemTemplate, AlternatingItemTemplate,
		// HeaderTemplate, FooterTemplate, SeparatorTemplate
		var allCounted = propNames.Concat(eventNames).ToList();
		allCounted.ShouldBeEmpty(
			$"Repeater should have no counted parameters. Got: {string.Join(", ", allCounted)}");

		skipped.Count(s => s.Contains("RenderFragment")).ShouldBeGreaterThanOrEqualTo(5,
			"Repeater should have ≥5 RenderFragment templates skipped");
	}

	#endregion

	#region AC-8: Obsolete params excluded (PRD §10.8 / §2.5)

	[Fact]
	public void ObsoleteParams_Excluded()
	{
		// Button overrides PostBackUrl with [Obsolete] — it should be skipped at Button level
		var (propNames, eventNames, skipped) = GetParameterDetails(typeof(BWF.Button));
		var allCounted = propNames.Concat(eventNames).ToList();

		// Button's PostBackUrl override is [Obsolete] — verify it's in the skipped list.
		// Note: it may be skipped as "[Obsolete]" or "no [Parameter]" depending on
		// whether the override redeclares [Parameter].
		skipped.ShouldContain(
			s => s.Contains("PostBackUrl"),
			"Button's PostBackUrl override should be in the skipped list");
	}

	[Fact]
	public void ObsoleteBaseParams_NeverCounted()
	{
		// Even if the stop-type mechanism were removed, [Obsolete] params should still be filtered.
		// Verify known obsolete params from BaseWebFormsComponent are not in any component's counts.
		var obsoleteParams = new[] { "runat", "EnableViewState", "DataKeys", "ItemPlaceholderID" };

		var (propNames, eventNames, _) = GetParameterDetails(typeof(BWF.Button));
		var allCounted = propNames.Concat(eventNames).ToList();

		foreach (var obsParam in obsoleteParams)
		{
			allCounted.ShouldNotContain(
				s => s.EndsWith($".{obsParam}"),
				$"Obsolete parameter '{obsParam}' should never be counted");
		}
	}

	#endregion

	#region AC-9: AdditionalAttributes excluded (PRD §10.9 / §2.5)

	[Fact]
	public void AdditionalAttributes_Excluded()
	{
		// AdditionalAttributes is on BaseWebFormsComponent (stop-type), but verify
		// the name-based exclusion works independently
		var typesToCheck = BwfAssembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract
				&& typeof(BWF.BaseWebFormsComponent).IsAssignableFrom(t))
			.Take(20);

		foreach (var type in typesToCheck)
		{
			var (propNames, eventNames, _) = GetParameterDetails(type);
			var allCounted = propNames.Concat(eventNames).ToList();

			allCounted.ShouldNotContain(
				s => s.EndsWith(".AdditionalAttributes"),
				$"AdditionalAttributes should never be counted for {CleanTypeName(type)}");
		}
	}

	#endregion

	#region AC-10: Intermediate base properties ARE counted (PRD §10.10)

	[Fact]
	public void IntermediateBase_PropertiesCounted()
	{
		// ButtonBaseComponent is between Button and BaseStyledComponent.
		// Its properties (Text, CausesValidation, etc.) MUST be counted for Button.
		var (propNames, _, _) = GetParameterDetails(typeof(BWF.Button));

		// These come from ButtonBaseComponent — must be in Button's property list
		propNames.ShouldContain(s => s.Contains("Text"),
			"ButtonBaseComponent.Text must be counted for Button");
		propNames.ShouldContain(s => s.Contains("CausesValidation"),
			"ButtonBaseComponent.CausesValidation must be counted for Button");
		propNames.ShouldContain(s => s.Contains("ValidationGroup"),
			"ButtonBaseComponent.ValidationGroup must be counted for Button");
		propNames.ShouldContain(s => s.Contains("CommandName"),
			"ButtonBaseComponent.CommandName must be counted for Button");
		propNames.ShouldContain(s => s.Contains("OnClientClick"),
			"ButtonBaseComponent.OnClientClick must be counted for Button");

		// UseSubmitBehavior comes from Button itself — also counted
		propNames.ShouldContain(s => s.Contains("UseSubmitBehavior"),
			"Button.UseSubmitBehavior must be counted");
	}

	[Fact]
	public void IntermediateBase_EventsCounted()
	{
		// OnClick and OnCommand from ButtonBaseComponent must appear as events for Button
		var (_, eventNames, _) = GetParameterDetails(typeof(BWF.Button));

		eventNames.ShouldContain(s => s.Contains("OnClick"),
			"ButtonBaseComponent.OnClick must be counted as event for Button");
		eventNames.ShouldContain(s => s.Contains("OnCommand"),
			"ButtonBaseComponent.OnCommand must be counted as event for Button");
	}

	#endregion

	#region AC-11: CascadingParameters excluded (PRD §10.11 / §2.5)

	[Fact]
	public void CascadingParameters_Excluded()
	{
		// ButtonBaseComponent has [CascadingParameter] Coordinator — but it's protected,
		// so it doesn't appear in Public | Instance | DeclaredOnly reflection at all.
		// The algorithm correctly excludes it because only public properties are scanned.
		// We verify it doesn't appear in counted results.
		var (propNames, eventNames, _) = GetParameterDetails(typeof(BWF.Button));
		var allCounted = propNames.Concat(eventNames).ToList();

		allCounted.ShouldNotContain(
			s => s.Contains("Coordinator"),
			"[CascadingParameter] Coordinator should not be counted");

		// Also verify CascadedTheme (on BaseWebFormsComponent) doesn't leak through
		allCounted.ShouldNotContain(
			s => s.Contains("CascadedTheme"),
			"[CascadingParameter] CascadedTheme should not be counted");
	}

	[Fact]
	public void CascadingParameters_ExcludedAcrossComponents()
	{
		// No cascading parameter should ever appear in counts for any component
		var typesToCheck = BwfAssembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract
				&& typeof(BWF.BaseWebFormsComponent).IsAssignableFrom(t))
			.Take(20);

		foreach (var type in typesToCheck)
		{
			var (propNames, eventNames, _) = GetParameterDetails(type);
			var allCounted = propNames.Concat(eventNames).ToList();

			// Check that no counted parameter has [CascadingParameter]
			var current = type;
			while (current != null && !IsStopType(current))
			{
				foreach (var prop in current.GetProperties(
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				{
					if (prop.GetCustomAttribute<CascadingParameterAttribute>() != null)
					{
						allCounted.ShouldNotContain(
							s => s.EndsWith($".{prop.Name}"),
							$"CascadingParameter '{prop.Name}' should not be counted for {CleanTypeName(type)}");
					}
				}
				current = current.BaseType;
			}
		}
	}

	#endregion

	#region Sanity checks — algorithm doesn't produce absurd results

	[Fact]
	public void SimpleComponents_CountsInSaneRange_Button()
	{
		var (props, events) = CountComponentSpecific(typeof(BWF.Button));
		(props + events).ShouldBeGreaterThan(0, "Button should have at least 1 counted parameter");
		(props + events).ShouldBeLessThan(30, $"Button count inflated ({props} props, {events} events)");
	}

	[Fact]
	public void SimpleComponents_CountsInSaneRange_Label()
	{
		var (props, events) = CountComponentSpecific(typeof(BWF.Label));
		(props + events).ShouldBeGreaterThan(0, "Label should have at least 1 counted parameter");
		(props + events).ShouldBeLessThan(30, $"Label count inflated ({props} props, {events} events)");
	}

	[Fact]
	public void SimpleComponents_CountsInSaneRange_TextBox()
	{
		var (props, events) = CountComponentSpecific(typeof(BWF.TextBox));
		(props + events).ShouldBeGreaterThan(0, "TextBox should have at least 1 counted parameter");
		(props + events).ShouldBeLessThan(30, $"TextBox count inflated ({props} props, {events} events)");
	}

	[Fact]
	public void SimpleComponents_CountsInSaneRange_Panel()
	{
		var (props, events) = CountComponentSpecific(typeof(BWF.Panel));
		(props + events).ShouldBeGreaterThan(0, "Panel should have at least 1 counted parameter");
		(props + events).ShouldBeLessThan(30, $"Panel count inflated ({props} props, {events} events)");
	}

	[Fact]
	public void StopTypes_AreNeverCounted()
	{
		// Direct verification: counting BaseWebFormsComponent itself should yield 0
		// (since it IS the stop type, the while loop doesn't execute)
		var (props, events) = CountComponentSpecific(typeof(BWF.BaseWebFormsComponent));
		props.ShouldBe(0, "BaseWebFormsComponent is a stop type — 0 properties expected");
		events.ShouldBe(0, "BaseWebFormsComponent is a stop type — 0 events expected");
	}

	#endregion
}
