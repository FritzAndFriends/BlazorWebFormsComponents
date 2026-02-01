using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Provides a base class for custom controls that contain child controls.
	/// This class allows Web Forms composite controls to be migrated to Blazor by providing
	/// a similar API surface to System.Web.UI.WebControls.CompositeControl.
	/// </summary>
	/// <example>
	/// <code>
	/// public class SearchBox : CompositeControl
	/// {
	///     private TextBox _textBox;
	///     private Button _button;
	///     
	///     protected override void CreateChildControls()
	///     {
	///         _textBox = new TextBox { ID = "searchQuery" };
	///         _button = new Button { ID = "searchButton", Text = "Search" };
	///         
	///         Controls.Add(_textBox);
	///         Controls.Add(_button);
	///     }
	///     
	///     protected override void Render(HtmlTextWriter writer)
	///     {
	///         writer.RenderBeginTag(HtmlTextWriterTag.Div);
	///         RenderChildren(writer);
	///         writer.RenderEndTag();
	///     }
	/// }
	/// </code>
	/// </example>
	public abstract class CompositeControl : WebControl
	{
		private bool _childControlsCreated;

		/// <summary>
		/// Gets a collection of child controls within the composite control.
		/// </summary>
		public new List<BaseWebFormsComponent> Controls { get; private set; } = new List<BaseWebFormsComponent>();

		/// <summary>
		/// Override this method to create and initialize child controls.
		/// This method is called automatically before rendering if child controls have not been created.
		/// </summary>
		protected virtual void CreateChildControls()
		{
			// Override in derived classes to add child controls
		}

		/// <summary>
		/// Ensures that child controls have been created by calling CreateChildControls if necessary.
		/// </summary>
		protected void EnsureChildControls()
		{
			if (!_childControlsCreated)
			{
				CreateChildControls();
				_childControlsCreated = true;
			}
		}

		/// <summary>
		/// Renders the child controls using the provided HtmlTextWriter.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to write output to.</param>
		protected void RenderChildren(HtmlTextWriter writer)
		{
			EnsureChildControls();

			foreach (var control in Controls)
			{
				if (control is WebControl webControl)
				{
					// Use the internal RenderControl method to render child controls
					webControl.RenderControl(writer);
				}
				else
				{
					// For other controls, we need to render them as Blazor components
					// This is a limitation - composite controls work best with WebControl children
					throw new NotSupportedException(
						$"CompositeControl.RenderChildren only supports child controls that inherit from WebControl. " +
						$"Control type '{control.GetType().Name}' is not supported.");
				}
			}
		}

		/// <summary>
		/// Renders the contents of the control by ensuring child controls are created
		/// and then calling RenderChildren.
		/// </summary>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			EnsureChildControls();
			RenderChildren(writer);
		}

		/// <summary>
		/// Builds the render tree for the Blazor component.
		/// </summary>
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			if (!Visible)
				return;

			EnsureChildControls();

			// For composite controls, we render using the standard Blazor approach
			// if there are child components, or fall back to HtmlTextWriter
			if (Controls.Count > 0 && Controls.TrueForAll(c => c is not WebControl))
			{
				// If all children are standard Blazor components, render them normally
				RenderChildrenAsBlazorComponents(builder);
			}
			else
			{
				// Otherwise, use the WebControl rendering approach
				base.BuildRenderTree(builder);
			}
		}

		/// <summary>
		/// Renders child controls as standard Blazor components.
		/// </summary>
		private void RenderChildrenAsBlazorComponents(RenderTreeBuilder builder)
		{
			var sequence = 0;

			// Render container with styles
			builder.OpenElement(sequence++, "div");

			if (!string.IsNullOrEmpty(Style))
			{
				builder.AddAttribute(sequence++, "style", Style);
			}

			if (!string.IsNullOrEmpty(CssClass))
			{
				builder.AddAttribute(sequence++, "class", CssClass);
			}

			if (!string.IsNullOrEmpty(ID))
			{
				builder.AddAttribute(sequence++, "id", ClientID);
			}

			// Render child content
			foreach (var control in Controls)
			{
				builder.OpenComponent(sequence++, control.GetType());
				builder.CloseComponent();
			}

			builder.CloseElement();
		}
	}
}
