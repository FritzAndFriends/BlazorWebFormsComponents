using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public abstract class BaseWebFormsComponent : ComponentBase, IAsyncDisposable
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

		#region Custom Events

		/// <summary>
		/// Event handler to mimic the Web Forms OnInit handler, and is triggered at the beginning of the OnInitialize Blazor event
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnInit { get; set; }

		/// <summary>
		/// Event handler to mimic the Web Forms OnLoad handler, and is triggered at the end of the OnInitialize Blazor event
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnLoad { get; set; }

		/// <summary>
		/// Event handler to mimic the Web Forms OnPreRender handler, and is triggered at the end of the OnInitialize Blazor event after Load
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnPreRender { get; set; }

		/// <summary>
		/// Event handler to mimic the Web Forms OnUnload handler, and is triggered at the end of the OnAfterRender Blazor event
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnUnload { get; set; }
		private bool _UnloadTriggered = false;

		/// <summary>
		/// Event handler to mimic the Web Forms OnDisposed handler and triggered in the Dispose method of this class
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnDisposed { get; set; }

		#endregion

		#region Blazor Events

		protected override async Task OnInitializedAsync()
		{

			if (OnInit.HasDelegate)
				await OnInit.InvokeAsync(EventArgs.Empty);

			await base.OnInitializedAsync();

			if (OnLoad.HasDelegate)
				await OnLoad.InvokeAsync(EventArgs.Empty);

			if (OnPreRender.HasDelegate)
				await OnPreRender.InvokeAsync(EventArgs.Empty);

		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			await base.OnAfterRenderAsync(firstRender);

			if (OnUnload.HasDelegate && !_UnloadTriggered)
			{
				await OnUnload.InvokeAsync(EventArgs.Empty);
				_UnloadTriggered = true;
			}

			if (firstRender)
			{

				HandleUnknownAttributes();
				StateHasChanged();

			}

		}

		protected virtual void HandleUnknownAttributes() { }


		#endregion

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual async ValueTask Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (OnDisposed.HasDelegate)
					{
						await OnDisposed.InvokeAsync(EventArgs.Empty);
					}
				}

				disposedValue = true;
			}
		}

		~BaseWebFormsComponent()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false).GetAwaiter().GetResult();
		}

		// This code added to correctly implement the disposable pattern.
		public ValueTask DisposeAsync()
		{
			GC.SuppressFinalize(this);
			return Dispose(true);
		}
		#endregion

		public bool LayoutTemplateRendered { get; set; } = false;



	}

}
