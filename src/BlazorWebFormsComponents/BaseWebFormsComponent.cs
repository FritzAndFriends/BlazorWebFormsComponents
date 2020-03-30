using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using IComponent = Microsoft.AspNetCore.Components.IComponent;

namespace BlazorWebFormsComponents
{

	public abstract class BaseWebFormsComponent : ComponentBase, IAsyncDisposable
	{

		#region Constructor

		private const string BASEFRAGMENTFIELDNAME = "_renderFragment";
		private const string PARENTCOMPONENTNAME = "ParentComponent";

		// Get Access to the ComponentBase field we need to wrap every component in a CascadingValue
		private static readonly FieldInfo _renderFragmentField = typeof(ComponentBase).GetField(BASEFRAGMENTFIELDNAME, BindingFlags.NonPublic | BindingFlags.Instance);
		private readonly RenderFragment _baseRenderFragment;

		public BaseWebFormsComponent()
		{
			// Grab a copy of the default RenderFragment to go into the CascadingValue
			_baseRenderFragment = (RenderFragment)_renderFragmentField.GetValue(this);

			// Override the default RenderFragment with our Special Sauce version
			_renderFragmentField.SetValue(this, (RenderFragment)ParentWrappingBuildRenderTree);

			void ParentWrappingBuildRenderTree(RenderTreeBuilder builder)
			{
				builder.OpenComponent(1, typeof(CascadingValue<BaseWebFormsComponent>));
				builder.AddAttribute(2, nameof(CascadingValue<object>.Name), PARENTCOMPONENTNAME);
				builder.AddAttribute(3, nameof(CascadingValue<object>.Value), this);
				builder.AddAttribute(4, nameof(CascadingValue<object>.ChildContent), _baseRenderFragment);
				builder.AddAttribute(5, nameof(CascadingValue<object>.IsFixed), true);
				builder.CloseComponent();
			}
		}

		#endregion

		#region Obsolete Attributes / Properties

		/// <summary>
		/// 🚨🚨 Use @ref instead of ID 🚨🚨
		/// </summary>
		[Parameter, Obsolete("Use @ref instead of ID")]
		public string ID { get; set; }

		/// <summary>
		/// While ViewState is supported by this library, this parameter does nothing
		/// </summary>
		[Parameter(), Obsolete("ViewState is supported, but EnableViewState does nothing")]
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

		[CascadingParameter(Name= PARENTCOMPONENTNAME)]
		public virtual BaseWebFormsComponent Parent { get; set; }

		[Parameter]
		public short TabIndex { get; set; }

		/// <summary>
		/// ViewState is supported for compatibility with those components and pages that add and retrieve items from ViewState.!--  It is not binary compatible, but is syntax compatible
		/// </summary>
		/// <value></value>
		[Obsolete("ViewState is supported for compatibility and is discouraged for future use")]
		public Dictionary<string,object> ViewState { get; } = new Dictionary<string, object>();

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

		[Parameter]
		public RenderFragment ChildComponents { get; set; }

		#region Blazor Events

		[Inject]
		public IJSRuntime JsRuntime { get; set; }

		protected override async Task OnInitializedAsync()
		{

			Parent?.Controls.Add(this);

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

				JsRuntime.InvokeVoidAsync("bwfc.Page.OnAfterRender", new object[] { });

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

		/// <summary>
		/// The list of child controls
		/// </summary>
		public List<BaseWebFormsComponent> Controls { get; set; } = new List<BaseWebFormsComponent>();

		/// <summary>
		/// Finds a child control by its ID
		/// </summary>
		/// <param name="controlId"> the ID of the child</param>
		/// <returns></returns>
		public BaseWebFormsComponent FindControl(string controlId)
		{
			return Controls.Find(control => control.ID == controlId);
		}

		protected event EventHandler BubbledEvent;
		protected virtual void OnBubbledEvent(object sender, EventArgs args) {

			BubbledEvent?.Invoke(sender, args);
			Parent?.OnBubbledEvent(sender,args);

		}

	}

}
