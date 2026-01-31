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
		/// Generates a client-side ID for a component based on its ID and parent hierarchy.
		/// Follows Web Forms naming container pattern: ParentID_ChildID
		/// </summary>
		/// <param name="component">The component to generate an ID for</param>
		/// <param name="suffix">Optional suffix to append (e.g., for child elements)</param>
		/// <returns>The generated client ID, or null if no ID is set</returns>
		public static string GetClientID(BaseWebFormsComponent component, string suffix = null)
		{
			if (component == null)
				return null;

			// If no ID is set, return null (don't generate IDs automatically)
			if (string.IsNullOrEmpty(component.ID))
				return null;

			var parts = new List<string>();

			// Walk up the parent hierarchy to build the full ID path
			var current = component;
			while (current != null)
			{
				if (!string.IsNullOrEmpty(current.ID))
				{
					parts.Insert(0, current.ID);
				}
				current = current.Parent;
			}

			var clientId = string.Join("_", parts);

			// Add suffix if provided (for child elements)
			if (!string.IsNullOrEmpty(suffix))
			{
				clientId = $"{clientId}_{suffix}";
			}

			return clientId;
		}

		/// <summary>
		/// Generates a client-side ID for a child element within a component.
		/// </summary>
		/// <param name="component">The parent component</param>
		/// <param name="childSuffix">The suffix for the child element</param>
		/// <returns>The generated client ID for the child element</returns>
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
		/// <returns>The generated client ID for the item</returns>
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
