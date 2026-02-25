using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using BlazorWebFormsComponents.Validations;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	public partial class TextBox : BaseStyledComponent, ITextComponent
	{
		[Parameter]
		public bool CausesValidation { get; set; } = true;

		[Parameter]
		public string ValidationGroup { get; set; }

		[CascadingParameter(Name = "ValidationGroupCoordinator")]
		protected ValidationGroupCoordinator Coordinator { get; set; }

		[Parameter]
		public string Text { get; set; } = string.Empty;

		[Parameter]
		public TextBoxMode TextMode { get; set; } = TextBoxMode.SingleLine;

		[Parameter]
		public int MaxLength { get; set; }

		[Parameter]
		public int Columns { get; set; }

		[Parameter]
		public int Rows { get; set; }

		[Parameter]
		public bool ReadOnly { get; set; }

		[Parameter]
		public string Placeholder { get; set; }

		[Parameter]
		public EventCallback<string> TextChanged { get; set; }

		[Parameter]
		public EventCallback<ChangeEventArgs> OnTextChanged { get; set; }

		[Parameter, Obsolete("AutoComplete is handled by browser settings")]
		public string AutoComplete { get; set; }

		[Parameter, Obsolete("AutoPostBack is not supported in Blazor")]
		public bool AutoPostBack { get; set; }

		internal string CalculatedType => TextMode switch
		{
			TextBoxMode.SingleLine => "text",
			TextBoxMode.Password => "password",
			TextBoxMode.Color => "color",
			TextBoxMode.Date => "date",
			TextBoxMode.DateTime => "datetime",
			TextBoxMode.DateTimeLocal => "datetime-local",
			TextBoxMode.Email => "email",
			TextBoxMode.Month => "month",
			TextBoxMode.Number => "number",
			TextBoxMode.Range => "range",
			TextBoxMode.Search => "search",
			TextBoxMode.Phone => "tel",
			TextBoxMode.Time => "time",
			TextBoxMode.Url => "url",
			TextBoxMode.Week => "week",
			_ => "text"
		};

		internal Dictionary<string, object> CalculatedAttributes
		{
			get
			{
				var attributes = new Dictionary<string, object>();

				if (!string.IsNullOrEmpty(ClientID))
					attributes["id"] = ClientID;

				if (!string.IsNullOrEmpty(CssClass))
					attributes["class"] = CssClass;

				if (!string.IsNullOrEmpty(Style))
					attributes["style"] = Style;

				if (!Enabled)
					attributes["disabled"] = true;

				if (ReadOnly)
					attributes["readonly"] = true;

				if (!string.IsNullOrEmpty(Placeholder))
					attributes["placeholder"] = Placeholder;

				if (TabIndex != 0)
					attributes["tabindex"] = TabIndex;

				if (!string.IsNullOrEmpty(ToolTip))
					attributes["title"] = ToolTip;

				if (TextMode == TextBoxMode.MultiLine)
				{
					if (Rows > 0)
						attributes["rows"] = Rows;
					if (Columns > 0)
						attributes["cols"] = Columns;
				}
				else
				{
					if (MaxLength > 0)
						attributes["maxlength"] = MaxLength;
					if (Columns > 0)
						attributes["size"] = Columns;
				}

				return attributes;
			}
		}
	}
}
