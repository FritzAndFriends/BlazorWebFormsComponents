using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorWebFormsComponents
{

	public abstract class BaseWebFormsComponent : ComponentBase
	{

		#region Obsolete Attributes / Properties

		/// <summary>
		/// 🚨🚨 Use @ref instead of ID 🚨🚨
		/// </summary>
		[Parameter, Obsolete("Use @ref instead of ID")]
		public string ID { get; set; }

		/// <summary>
		/// 🚨🚨 ViewState is not available in Blazor 🚨🚨
		/// </summary>
		[Parameter(), Obsolete("ViewState is not available in Blazor")]
		public bool EnableViewState { get; set; }

		/// <summary>
		/// 🚨🚨 runat is not available in Blazor 🚨🚨
		/// </summary>
		[Parameter(), Obsolete("runat is not available in Blazor")]
		public string runat { get; set; }

		/// <summary>
		/// 🚨🚨 DataKeys are not used in Blazor 🚨🚨
		/// </summary>
		[Parameter(), Obsolete("DataKeys are not used in Blazor")]
		public string DataKeys { get; set; }

		/// <summary>
		/// 🚨🚨 DataSource controls are not used in Blazor 🚨🚨
		/// </summary>
		[Parameter, Obsolete("DataSource controls are not used in Blazor")]
		public string DataSourceID { get; set; }

		/// <summary>
		/// 🚨🚨 Theming is not available in Blazor 🚨🚨
		/// </summary>
		[Parameter, Obsolete("Theming is not available in Blazor")]
		public bool EnableTheming { get; set; }

		/// <summary>
		/// 🚨🚨 Theming is not available in Blazor 🚨🚨
		/// </summary>
		[Parameter, Obsolete("Theming is not available in Blazor")]
		public bool SkinID { get; set; }

		#endregion

		[Parameter]
		public bool Enabled { get; set; } = true;

		[Parameter]
		public short TabIndex { get; set; }

		/// <summary>
		/// Is the content of this component rendered and visible to your users?
		/// </summary>
		[Parameter]
		public bool Visible { get; set; } = true;

    [Obsolete("This method doesn't do anything in Blazor")]
    public void DataBind() { }

		/// <summary>
		/// 🚨🚨 Placeholders are not available in Blazor 🚨🚨
		/// </summary>
		[Parameter, Obsolete("Placeholders are not available in Blazor")]

		public string ItemPlaceholderID { get; set; }


		[Parameter(CaptureUnmatchedValues = true)]
		public Dictionary<string, object> AdditionalAttributes { get; set; }

	}

}
