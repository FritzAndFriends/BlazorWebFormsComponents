using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Linq;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A button to be placed inside of a GridView
	/// </summary>
	public partial class ButtonField<ItemType> : BaseColumn<ItemType>
	{

		[Parameter] public ButtonType ButtonType { get; set; }

		[Parameter] public string CommandName { get; set; }

		[Parameter] public string DataTextField { get; set; }
		/// <summary>
		/// The format string to apply to the data specified by the <see cref="DataTextField"/> property.
		/// </summary>
		[Parameter] public string DataTextFormatString { get; set; }

		/// <summary>
		/// Gets or sets the image to display for each button in the ButtonField object.
		/// </summary>
		[Parameter] public string ImageUrl { get; set; }

		internal int DataItemIndex { get; set; }


		/// <summary>
		/// The text for the hyperlink. If <see cref="DataTextFormatString"/> is specified, that will override the value of this property.
		/// </summary>
		[Parameter] public string Text { get; set; }

		public override RenderFragment Render(ItemType item)
		{
			string text;
			if (string.IsNullOrEmpty(DataTextFormatString))
			{
				text = Text;
			}
			else
			{
				var textArgs = GetDataFields(item, DataTextField);
				text = string.Format(CultureInfo.CurrentCulture, DataTextFormatString, textArgs);
			}

			switch (ButtonType) {
				case ButtonButtonType _:
					return RenderButton(text);
				case ImageButtonType _:
					return RenderImage();
				default:
					return RenderLink(text);
			}

		}

		public void OnCommand(object src, string commandName, object commandArg) {

			var handler = (base.ParentColumnsCollection as GridView<ItemType>)?.OnRowCommand;
			if (handler == null) return;

			handler.Value.InvokeAsync(new GridViewCommandEventArgs {
				CommandArgument = commandArg,
				CommandName = commandName,
				CommandSource = src
			}).GetAwaiter().GetResult();

		}

		private object[] GetDataFields(ItemType item, string dataFieldNames)
		{
			var dataFields = dataFieldNames.Split(',').Select(s => s.Trim()).ToList();
			var fields = new object[dataFields.Count];
			for (var i = 0; i < dataFields.Count; i++)
			{
				fields[i] = DataBinder.GetPropertyValue(item, dataFields[i]);
			}
			return fields;
		}
	}

	public class GridViewCommandEventArgs {

		public object CommandArgument { get; set; }

		public string CommandName { get; set; }

		public object CommandSource { get; set; }

		public bool Handled { get; set; }

	}
}
