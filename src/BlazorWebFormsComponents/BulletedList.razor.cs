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
	/// <typeparam name="TItem">The type of items in the data source.</typeparam>
	public partial class BulletedList<TItem> : DataBoundComponent<TItem>, IStyle
	{
		private readonly string _baseId = Guid.NewGuid().ToString("N").Substring(0, 8);

		/// <summary>
		/// Gets or sets the bullet style of the list.
		/// </summary>
		[Parameter]
		public BulletStyle BulletStyle { get; set; } = BulletStyle.NotSet;

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
		public BulletedListDisplayMode DisplayMode { get; set; } = BulletedListDisplayMode.Text;

		/// <summary>
		/// Gets or sets the starting number for a numbered list.
		/// </summary>
		[Parameter]
		public int FirstBulletNumber { get; set; } = 1;

		/// <summary>
		/// Gets or sets the collection of static list items.
		/// </summary>
		[Parameter]
		public ListItemCollection StaticItems { get; set; } = new();

		/// <summary>
		/// Gets or sets the field of the data source that provides the text content of the list items.
		/// </summary>
		[Parameter]
		public string DataTextField { get; set; }

		/// <summary>
		/// Gets or sets the field of the data source that provides the value of each list item.
		/// </summary>
		[Parameter]
		public string DataValueField { get; set; }

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

		// IStyle implementation
		[Parameter]
		public WebColor BackColor { get; set; }

		[Parameter]
		public WebColor BorderColor { get; set; }

		[Parameter]
		public BorderStyle BorderStyle { get; set; }

		[Parameter]
		public Unit BorderWidth { get; set; }

		[Parameter]
		public string CssClass { get; set; }

		[Parameter]
		public FontInfo Font { get; set; } = new FontInfo();

		[Parameter]
		public WebColor ForeColor { get; set; }

		[Parameter]
		public Unit Height { get; set; }

		[Parameter]
		public Unit Width { get; set; }

		/// <summary>
		/// Gets the computed style string for the component.
		/// </summary>
		protected string Style => this.ToStyle().NullIfEmpty();

		/// <summary>
		/// Gets a value indicating whether the bullet style renders as an ordered list.
		/// </summary>
		protected bool IsOrderedList => BulletStyle switch
		{
			BulletStyle.Numbered => true,
			BulletStyle.LowerAlpha => true,
			BulletStyle.UpperAlpha => true,
			BulletStyle.LowerRoman => true,
			BulletStyle.UpperRoman => true,
			_ => false
		};

		/// <summary>
		/// Gets the HTML list-style-type value for the current bullet style.
		/// </summary>
		protected string ListStyleType => BulletStyle switch
		{
			BulletStyle.Disc => "disc",
			BulletStyle.Circle => "circle",
			BulletStyle.Square => "square",
			BulletStyle.Numbered => "decimal",
			BulletStyle.LowerAlpha => "lower-alpha",
			BulletStyle.UpperAlpha => "upper-alpha",
			BulletStyle.LowerRoman => "lower-roman",
			BulletStyle.UpperRoman => "upper-roman",
			BulletStyle.CustomImage => null,
			_ => null
		};

		/// <summary>
		/// Gets the HTML type attribute value for ordered lists.
		/// </summary>
		protected string OrderedListType => BulletStyle switch
		{
			BulletStyle.Numbered => "1",
			BulletStyle.LowerAlpha => "a",
			BulletStyle.UpperAlpha => "A",
			BulletStyle.LowerRoman => "i",
			BulletStyle.UpperRoman => "I",
			_ => null
		};

		/// <summary>
		/// Gets the combined style string including list-style customization.
		/// </summary>
		protected string CombinedStyle
		{
			get
			{
				var baseStyle = Style ?? string.Empty;
				
				if (BulletStyle == BulletStyle.CustomImage && !string.IsNullOrEmpty(BulletImageUrl))
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
				await OnClick.InvokeAsync(new BulletedListEventArgs(index));
			}
		}

		/// <summary>
		/// Gets all items from both static and data-bound sources.
		/// </summary>
		protected IEnumerable<ListItem> GetItems()
		{
			// Return static items first
			foreach (var item in StaticItems)
			{
				yield return item;
			}

			// Then data-bound items
			if (Items != null)
			{
				foreach (var dataItem in Items)
				{
					yield return new ListItem
					{
						Text = GetPropertyValue(dataItem, DataTextField),
						Value = GetPropertyValue(dataItem, DataValueField)
					};
				}
			}
		}

		private string GetPropertyValue(TItem item, string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				return item?.ToString() ?? string.Empty;

			var prop = typeof(TItem).GetProperty(propertyName);
			return prop?.GetValue(item)?.ToString() ?? string.Empty;
		}
	}
}
