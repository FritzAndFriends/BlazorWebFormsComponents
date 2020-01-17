using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWebFormsComponents.Validations
{
	public abstract partial class BaseValidator<Type> : BaseWebFormsComponent, IHasStyle
	{
		[CascadingParameter] EditContext CurrentEditContext { get; set; }

		[Parameter] public ForwardRef<InputBase<Type>> ControlToValidate { get; set; }
		[Parameter] public string Text { get; set; }
		[Parameter] public string ErrorMessage { get; set; }
		[Parameter] public WebColor ForeColor { get; set; }

		protected bool IsValid { get; set; } = true;

		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public HorizontalAlign HorizontalAlign { get; set; }
		[Parameter] public VerticalAlign VerticalAlign { get; set; }
		[Parameter] public Unit Width { get; set; }
		[Parameter] public bool Font_Bold { get; set; }
		[Parameter] public bool Font_Italic { get; set; }
		[Parameter] public string Font_Names { get; set; }
		[Parameter] public bool Font_Overline { get; set; }
		[Parameter] public FontUnit Font_Size { get; set; }
		[Parameter] public bool Font_Strikeout { get; set; }
		[Parameter] public bool Font_Underline { get; set; }

		/// <summary>
		/// Override all style properties if it's not null
		/// </summary>
		[Parameter]
		public IHasStyle Style
		{
			get => this;
			set { value?.CopyTo(this); }
		}

		protected string CalculatedStyle => this.ToStyleString();

		protected override void OnInitialized()
		{

			var messages = new ValidationMessageStore(CurrentEditContext);

			CurrentEditContext.OnValidationRequested += (sender, eventArgs) => EventHandler((EditContext)sender, messages);

			this.SetFontsFromAttributes(AdditionalAttributes);

			base.OnInitialized();

		}

		protected override void OnParametersSet()
		{



			base.OnParametersSet();

		}

		public void EventHandler(EditContext editContext, ValidationMessageStore messages)
		{
			string name;
			if (ControlToValidate.Current.ValueExpression.Body is MemberExpression memberExpression)
			{
				name = memberExpression.Member.Name;
			}
			else
			{
				throw new InvalidOperationException("You shoud not have seen this message, but now that you do" +
					"I want you to open an issue here https://github.com/fritzAndFriends/BlazorWebFormsComponents/issues " +
					"with a title 'ValueExpression.Body is not MemberExpression' and a sample code to reproduce this. Thanks!");
			}

			var fieldIdentifier = CurrentEditContext.Field(name);

			messages.Clear(fieldIdentifier);

			var value = typeof(InputBase<Type>).GetProperty("CurrentValueAsString", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ControlToValidate.Current) as string;

			if (!Enabled || Validate(value))
			{
				IsValid = true;
			}
			else
			{
				IsValid = false;
				// Text is for validator,ErrorMessage is for validation summary
				messages.Add(fieldIdentifier, Text + "," + ErrorMessage);
			}

			editContext.NotifyValidationStateChanged();
		}

		public abstract bool Validate(string value);
	}
}
