namespace BlazorWebFormsComponents;

/// <summary>
/// Represents an item in a list control such as DropDownList, CheckBoxList, RadioButtonList, or ListBox.
/// </summary>
public class ListItem
{
	/// <summary>
	/// Gets or sets the text displayed in the list control for the item.
	/// </summary>
	public string Text { get; set; }

	/// <summary>
	/// Gets or sets the value associated with the list item.
	/// </summary>
	public string Value { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the list item is selected.
	/// </summary>
	public bool Selected { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the list item is enabled.
	/// </summary>
	public bool Enabled { get; set; } = true;

	/// <summary>
	/// Initializes a new instance of the <see cref="ListItem"/> class.
	/// </summary>
	public ListItem() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="ListItem"/> class with the specified text.
	/// The value is set to the same as the text.
	/// </summary>
	/// <param name="text">The text to display for the item.</param>
	public ListItem(string text) : this(text, text) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="ListItem"/> class with the specified text and value.
	/// </summary>
	/// <param name="text">The text to display for the item.</param>
	/// <param name="value">The value associated with the item.</param>
	public ListItem(string text, string value)
	{
		Text = text;
		Value = value;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ListItem"/> class with the specified text, value, and selected state.
	/// </summary>
	/// <param name="text">The text to display for the item.</param>
	/// <param name="value">The value associated with the item.</param>
	/// <param name="selected">Whether the item is selected.</param>
	public ListItem(string text, string value, bool selected)
	{
		Text = text;
		Value = value;
		Selected = selected;
	}
}
