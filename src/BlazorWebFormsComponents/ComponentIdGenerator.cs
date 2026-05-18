using BlazorWebFormsComponents.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Utility class for generating HTML element IDs for Blazor Web Forms components.
	/// Follows ASP.NET Web Forms ClientID patterns for compatibility with legacy JavaScript.
	/// </summary>
	public static class ComponentIdGenerator
	{
		/// <summary>
		/// Resolves the effective ClientIDMode for a component by walking up the parent hierarchy.
		/// If no ancestor specifies a non-Inherit mode, defaults to Predictable (matches Web Forms page-level default).
		/// </summary>
		public static ClientIDMode GetEffectiveClientIDMode(BaseWebFormsComponent component)
		{
			var current = component;
			while (current != null)
			{
				if (current.ClientIDMode != ClientIDMode.Inherit)
					return current.ClientIDMode;
				current = current.Parent;
			}
			// No ancestor specified a mode — default to Predictable (Web Forms page default)
			return ClientIDMode.Predictable;
		}

		/// <summary>
		/// Generates a client-side ID for a component based on its ID and parent hierarchy.
		/// Follows Web Forms naming container pattern: ParentID_ChildID
		/// </summary>
		/// <param name="component">The component to generate an ID for</param>
		/// <param name="suffix">Optional suffix to append (e.g., for child elements)</param>
		/// <returns>The generated client ID, or null if component is null or has no ID set</returns>
		public static string GetClientID(BaseWebFormsComponent component, string suffix = null)
		{
			if (component == null)
				return null;

			// If no ID is set, return null (don't generate IDs automatically)
			if (string.IsNullOrEmpty(component.ID))
				return null;

			var effectiveMode = GetEffectiveClientIDMode(component);

			string clientId;

			switch (effectiveMode)
			{
				case ClientIDMode.Static:
					// Static: raw ID, no parent walking
					clientId = component.ID;
					break;

				case ClientIDMode.AutoID:
					// AutoID: walk parents, include ctl00 prefixes from NamingContainers
					clientId = BuildAutoID(component);
					break;

				case ClientIDMode.Predictable:
				default:
					// Predictable: walk parents, join with underscore, skip ctl00 prefixes
					clientId = BuildPredictableID(component);
					break;
			}

			// Add suffix if provided (for child elements)
			if (!string.IsNullOrEmpty(suffix))
			{
				clientId = $"{clientId}_{suffix}";
			}

			return clientId;
		}

		/// <summary>
		/// Builds the AutoID-style client ID: walks parents and includes ctl00 prefixes
		/// from NamingContainers that have UseCtl00Prefix set.
		/// </summary>
		private static string BuildAutoID(BaseWebFormsComponent component)
		{
			var parts = new List<string>();

			var current = component;
			while (current != null)
			{
				if (!string.IsNullOrEmpty(current.ID))
				{
					parts.Insert(0, current.ID);
				}
				// ctl00 prefix only applies in AutoID mode
				if (current is NamingContainer nc && nc.UseCtl00Prefix)
				{
					parts.Insert(0, "ctl00");
				}
				current = current.Parent;
			}

			return string.Join("_", parts);
		}

		/// <summary>
		/// Builds the Predictable-style client ID: walks parents and joins IDs with underscores,
		/// but does not include ctl00/ctlxxx prefixes.
		/// </summary>
		private static string BuildPredictableID(BaseWebFormsComponent component)
		{
			var parts = new List<string>();

			var current = component;
			while (current != null)
			{
				if (!string.IsNullOrEmpty(current.ID))
				{
					parts.Insert(0, current.ID);
				}
				current = current.Parent;
			}

			return string.Join("_", parts);
		}

		/// <summary>
		/// Generates a UniqueID for a component — the form submission name.
		/// Uses '$' as separator (matching Web Forms UniqueID behavior).
		/// ClientID uses '_' for HTML id attributes; UniqueID uses '$' for form name attributes.
		/// </summary>
		/// <param name="component">The component to generate a UniqueID for</param>
		/// <returns>The generated unique ID, or null if component is null or has no ID set</returns>
		public static string GetUniqueID(BaseWebFormsComponent component)
		{
			if (component == null || string.IsNullOrEmpty(component.ID))
				return null;

			// Check for a cascaded FormNamingContext first — this provides
			// row-level naming from GridViewRow or other naming containers
			// that can't participate in the Blazor component parent chain.
			// The context prefix already includes the grid's UniqueID and ctl{nn}.
			// We just append the component's own ID.
			// Note: FormNamingContext is set on the component by the cascading parameter handler.

			var parts = new List<string>();
			var current = component;
			while (current != null)
			{
				if (!string.IsNullOrEmpty(current.ID))
				{
					parts.Insert(0, current.ID);
				}
				if (current is NamingContainer nc && nc.UseCtl00Prefix)
				{
					parts.Insert(0, "ctl00");
				}
				current = current.Parent;
			}

			return string.Join("$", parts);
		}

		/// <summary>
		/// Generates a UniqueID for a component within a naming context (e.g., inside a GridView row).
		/// The naming context prefix is prepended to the component's own ID.
		/// </summary>
		/// <param name="namingContextPrefix">The prefix from the naming container (e.g., "GridView1$ctl02")</param>
		/// <param name="componentId">The component's own ID</param>
		/// <returns>The full unique ID (e.g., "GridView1$ctl02$PurchaseQuantity")</returns>
		public static string GetUniqueIDWithContext(string namingContextPrefix, string componentId)
		{
			if (string.IsNullOrEmpty(componentId))
				return null;
			if (string.IsNullOrEmpty(namingContextPrefix))
				return componentId;
			return $"{namingContextPrefix}${componentId}";
		}

		/// <summary>
		/// Generates a client-side ID for a child element within a component.
		/// </summary>
		/// <param name="component">The parent component</param>
		/// <param name="childSuffix">The suffix for the child element</param>
		/// <returns>The generated client ID for the child element, or null if component is null or has no ID set</returns>
		public static string GetChildClientID(BaseWebFormsComponent component, string childSuffix)
		{
			return GetClientID(component, childSuffix);
		}

		/// <summary>
		/// Generates a client-side ID for an item in a data-bound control (e.g., GridView row, TreeView node).
		/// </summary>
		/// <param name="component">The parent component</param>
		/// <param name="itemIndex">The index of the item</param>
		/// <param name="suffix">Optional additional suffix</param>
		/// <returns>The generated client ID for the item, or null if component is null or has no ID set</returns>
		public static string GetItemClientID(BaseWebFormsComponent component, int itemIndex, string suffix = null)
		{
			var baseSuffix = itemIndex.ToString();
			if (!string.IsNullOrEmpty(suffix))
			{
				baseSuffix = $"{baseSuffix}_{suffix}";
			}
			return GetClientID(component, baseSuffix);
		}
	}
}
