using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWebFormsComponents.Validations
{
	public partial class AspNetValidationSummary : BaseWebFormsComponent, IHasStyle, IDisposable
	{
		private EditContext _previousEditContext;
		private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;

		[CascadingParameter] EditContext CurrentEditContext { get; set; }

		[Parameter] public ValidationSummaryDisplayMode DisplayMode { get; set; } = ValidationSummaryDisplayMode.BulletList;
		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public WebColor ForeColor { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public Unit Width { get; set; }
		[Parameter] public bool Font_Bold { get; set; }
		[Parameter] public bool Font_Italic { get; set; }
		[Parameter] public string Font_Names { get; set; }
		[Parameter] public bool Font_Overline { get; set; }
		[Parameter] public FontUnit Font_Size { get; set; }
		[Parameter] public bool Font_Strikeout { get; set; }
		[Parameter] public bool Font_Underline { get; set; }

		protected StyleBuilder CalculatedStyle { get; set; }

		public bool IsValid => CurrentEditContext.GetValidationMessages().Any();

		public IEnumerable<string> ValidationMessages => CurrentEditContext.GetValidationMessages().Select(x => x.Split(',')[1]);

		public AspNetValidationSummary()
		{
			_validationStateChangedHandler = (sender, eventArgs) => StateHasChanged();
		}

		protected override void OnParametersSet()
		{

			if (CurrentEditContext == null)
			{
				throw new InvalidOperationException($"{nameof(ValidationSummary)} requires a cascading parameter " +
						$"of type {nameof(EditContext)}. For example, you can use {nameof(ValidationSummary)} inside " +
						$"an {nameof(EditForm)}.");
			}

			if (CurrentEditContext != _previousEditContext)
			{
				DetachValidationStateChangedListener();
				CurrentEditContext.OnValidationStateChanged += _validationStateChangedHandler;
				_previousEditContext = CurrentEditContext;
			}
		}

		protected override void OnInitialized()
		{

			this.SetFontsFromAttributes(AdditionalAttributes);

			CalculatedStyle = this.ToStyle();

			base.OnInitialized();

		}

		public void Dispose()
		{
			DetachValidationStateChangedListener();
		}

		private void DetachValidationStateChangedListener()
		{
			if (_previousEditContext != null)
			{
				_previousEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
			}
		}
	}
}
