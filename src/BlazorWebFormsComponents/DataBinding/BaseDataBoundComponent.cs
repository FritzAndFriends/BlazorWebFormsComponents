using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents.DataBinding
{
	public class BaseDataBoundComponent : BaseStyledComponent
	{
		[Parameter]
		public virtual object DataSource { get; set; }

		[Parameter, Obsolete("DataSource controls are not used in Blazor")]
		public string DataSourceID { get; set; }

		[Parameter]
		public EventCallback<EventArgs> OnDataBound { get; set; }

		protected virtual void DataBound(EventArgs e)
		{
			OnDataBound.InvokeAsync(e);
		}
	}
}
