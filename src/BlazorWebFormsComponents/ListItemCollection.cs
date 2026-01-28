using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents;

/// <summary>
/// Represents a collection of <see cref="ListItem"/> objects.
/// </summary>
public class ListItemCollection : List<ListItem>
{
	/// <summary>
	/// Finds the first list item with the specified value.
	/// </summary>
	/// <param name="value">The value to search for.</param>
	/// <returns>The first <see cref="ListItem"/> with the specified value, or null if not found.</returns>
	public ListItem FindByValue(string value)
	{
		return this.FirstOrDefault(i => i.Value == value);
	}

	/// <summary>
	/// Finds the first list item with the specified text.
	/// </summary>
	/// <param name="text">The text to search for.</param>
	/// <returns>The first <see cref="ListItem"/> with the specified text, or null if not found.</returns>
	public ListItem FindByText(string text)
	{
		return this.FirstOrDefault(i => i.Text == text);
	}
}
