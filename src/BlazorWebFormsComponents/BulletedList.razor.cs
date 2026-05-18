using BlazorComponentUtilities;
using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
/// <summary>
/// Represents a list control that displays items as a bulleted or numbered list.
/// Emulates the ASP.NET Web Forms BulletedList control.
/// </summary>
/// <typeparam name="ItemType">The type of items in the data source.</typeparam>
public partial class BulletedList<ItemType> : BaseListControl<ItemType>
{
private readonly string _baseId = Guid.NewGuid().ToString("N").Substring(0, 8);

/// <summary>
/// Gets or sets a value indicating whether validation is performed when the control posts back.
/// </summary>
[Parameter]
public bool CausesValidation { get; set; } = true;

/// <summary>
/// Gets or sets the group of controls for which the control causes validation.
/// </summary>
[Parameter]
public string ValidationGroup { get; set; }

/// <summary>
/// Gets or sets the bullet style of the list.
/// </summary>
[Parameter]
public EnumParameter<BulletStyle> BulletStyle { get; set; } = Enums.BulletStyle.NotSet;

/// <summary>
/// Gets or sets the URL of the custom bullet image.
/// Only used when BulletStyle is set to CustomImage.
/// </summary>
[Parameter]
public string BulletImageUrl { get; set; }

/// <summary>
/// Gets or sets the display mode of the list items.
/// </summary>
[Parameter]
public EnumParameter<BulletedListDisplayMode> DisplayMode { get; set; } = BulletedListDisplayMode.Text;

/// <summary>
/// Gets or sets the starting number for a numbered list.
/// </summary>
[Parameter]
public int FirstBulletNumber { get; set; } = 1;

/// <summary>
/// Gets or sets the target window or frame for hyperlinks when DisplayMode is HyperLink.
/// </summary>
[Parameter]
public string Target { get; set; }

/// <summary>
/// Gets or sets the event callback that is invoked when an item is clicked in LinkButton mode.
/// </summary>
[Parameter]
public EventCallback<BulletedListEventArgs> OnClick { get; set; }

/// <summary>
/// Blazor event alias for the Web Forms Click event.
/// </summary>
[Parameter]
public EventCallback<BulletedListEventArgs> Click { get; set; }

/// <summary>
/// Occurs when the selection changes. Migration stub for Web Forms postback event.
/// </summary>
[Parameter]
public EventCallback<EventArgs> SelectedIndexChanged { get; set; }

/// <summary>
/// Occurs when the text changes. Migration stub for Web Forms postback event.
/// </summary>
[Parameter]
public EventCallback<EventArgs> TextChanged { get; set; }

/// <summary>
/// Gets or sets whether the control automatically posts back when selection changes.
/// BulletedList is a display-only control that does not support AutoPostBack.
/// </summary>
[Parameter, Obsolete("BulletedList does not support AutoPostBack. Use OnClick event instead.")]
public bool AutoPostBack { get; set; }

/// <summary>
/// Gets or sets the index of the selected item. -1 when no item is selected.
/// </summary>
[Parameter]
public int SelectedIndex { get; set; } = -1;

/// <summary>
/// Gets or sets the value of the selected item.
/// </summary>
[Parameter]
public string SelectedValue { get; set; }

/// <summary>
/// Gets or sets the text caption for the control. Migration stub for ListControl.Text.
/// </summary>
[Parameter]
public string Text { get; set; }

/// <summary>
/// Gets a value indicating whether the bullet style renders as an ordered list.
/// </summary>
protected bool IsOrderedList => BulletStyle.Value switch
{
Enums.BulletStyle.Numbered => true,
Enums.BulletStyle.LowerAlpha => true,
Enums.BulletStyle.UpperAlpha => true,
Enums.BulletStyle.LowerRoman => true,
Enums.BulletStyle.UpperRoman => true,
_ => false
};

/// <summary>
/// Gets the HTML list-style-type value for the current bullet style.
/// </summary>
protected string ListStyleType => BulletStyle.Value switch
{
Enums.BulletStyle.Disc => "disc",
Enums.BulletStyle.Circle => "circle",
Enums.BulletStyle.Square => "square",
Enums.BulletStyle.Numbered => "decimal",
Enums.BulletStyle.LowerAlpha => "lower-alpha",
Enums.BulletStyle.UpperAlpha => "upper-alpha",
Enums.BulletStyle.LowerRoman => "lower-roman",
Enums.BulletStyle.UpperRoman => "upper-roman",
Enums.BulletStyle.CustomImage => null,
_ => null
};

/// <summary>
/// Gets the start attribute value for ordered lists, or null when the default (1) is used.
/// WebForms only renders the start attribute when FirstBulletNumber differs from 1.
/// </summary>
protected int? GetStartAttribute() =>
	IsOrderedList && FirstBulletNumber != 1 ? FirstBulletNumber : null;

/// <summary>
/// Gets the combined style string including list-style customization.
/// </summary>
protected string CombinedStyle
{
get
{
var baseStyle = Style ?? string.Empty;

if (BulletStyle.Value == Enums.BulletStyle.CustomImage && !string.IsNullOrEmpty(BulletImageUrl))
{
var imageStyle = $"list-style-image: url('{BulletImageUrl}');";
return string.IsNullOrEmpty(baseStyle) ? imageStyle : $"{baseStyle} {imageStyle}";
}

if (ListStyleType != null)
{
var typeStyle = $"list-style-type: {ListStyleType};";
return string.IsNullOrEmpty(baseStyle) ? typeStyle : $"{baseStyle} {typeStyle}";
}

return string.IsNullOrEmpty(baseStyle) ? null : baseStyle;
}
}

/// <summary>
/// Handles the click event for an item in LinkButton mode.
/// </summary>
protected async Task HandleItemClick(int index)
{
if (DisplayMode == BulletedListDisplayMode.LinkButton && Enabled)
{
	var args = new BulletedListEventArgs(index);
	var clickHandler = OnClick.HasDelegate ? OnClick : Click;
	await clickHandler.InvokeAsync(args);
}
}
}
}