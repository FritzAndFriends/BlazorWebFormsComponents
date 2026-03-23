using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.CustomControls
{
    /// <summary>
    /// Provides a base class for custom controls that combine HtmlTextWriter rendering
    /// (TagKey, AddAttributesToRender, RenderContents) with data binding (DataSource, PerformDataBinding).
    /// This is the CustomControls equivalent of inheriting from System.Web.UI.WebControls.DataBoundControl
    /// in Web Forms.
    /// </summary>
    /// <example>
    /// <code>
    /// public class EmployeeList : DataBoundWebControl
    /// {
    ///     protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Table;
    ///     
    ///     protected override void PerformDataBinding(IEnumerable data)
    ///     {
    ///         // Store or transform data for rendering
    ///     }
    ///     
    ///     protected override void RenderContents(HtmlTextWriter writer)
    ///     {
    ///         // Render table rows from bound data
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class DataBoundWebControl : WebControl
    {
        /// <summary>
        /// Gets or sets the data source for this control.
        /// </summary>
        [Parameter]
        public virtual object DataSource { get; set; }

        /// <summary>
        /// Gets or sets the ID of the data source control. Not used in Blazor.
        /// </summary>
        [Parameter, Obsolete("DataSourceID is not used in Blazor. Use the DataSource parameter instead.")]
        public virtual string DataSourceID { get; set; }

        /// <summary>
        /// Gets or sets the data member to use when binding.
        /// </summary>
        [Parameter]
        public virtual string DataMember { get; set; }

        /// <summary>
        /// Event raised after data binding is complete.
        /// </summary>
        [Parameter]
        public EventCallback<EventArgs> OnDataBound { get; set; }

        /// <summary>
        /// Gets the enumerable data items after binding.
        /// </summary>
        protected IEnumerable DataItems { get; private set; }

        /// <summary>
        /// Override this method to process the data source when it is bound.
        /// This is the primary extensibility point for data-bound custom controls.
        /// </summary>
        /// <param name="data">The enumerable data from the DataSource.</param>
        protected virtual void PerformDataBinding(IEnumerable data)
        {
            // Default implementation does nothing — subclasses override
        }

        /// <summary>
        /// Performs data binding when the DataSource parameter changes.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (DataSource != null)
            {
                var enumerable = DataSource as IEnumerable;
                DataItems = enumerable;
                PerformDataBinding(enumerable);
                OnDataBound.InvokeAsync(EventArgs.Empty);
            }
            else
            {
                DataItems = null;
            }
        }
    }

    /// <summary>
    /// Provides a strongly-typed base class for custom controls that combine HtmlTextWriter
    /// rendering with generic data binding.
    /// </summary>
    /// <typeparam name="T">The type of data items in the data source.</typeparam>
    /// <example>
    /// <code>
    /// public class EmployeeGrid : DataBoundWebControl&lt;Employee&gt;
    /// {
    ///     protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Table;
    ///     
    ///     protected override void RenderContents(HtmlTextWriter writer)
    ///     {
    ///         foreach (var emp in TypedDataItems)
    ///         {
    ///             writer.RenderBeginTag(HtmlTextWriterTag.Tr);
    ///             writer.RenderBeginTag(HtmlTextWriterTag.Td);
    ///             writer.Write(emp.Name);
    ///             writer.RenderEndTag();
    ///             writer.RenderEndTag();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class DataBoundWebControl<T> : DataBoundWebControl
    {
        /// <summary>
        /// Gets the strongly-typed data items after binding.
        /// Blazor sets the base <see cref="DataBoundWebControl.DataSource"/> parameter directly;
        /// use this property to access the data with compile-time type safety.
        /// </summary>
        protected IEnumerable<T> TypedDataItems =>
            base.DataItems?.Cast<T>() ?? Enumerable.Empty<T>();
    }
}
