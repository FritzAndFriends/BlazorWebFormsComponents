using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A component that emulates ASP.NET Web Forms MasterPage functionality.
	/// Wrap your master-page chrome (header, nav, footer) inside this component and use
	/// <see cref="ContentPlaceHolder"/> controls to define named content slots. Child
	/// pages provide slot content via <see cref="Content"/> controls.
	/// </summary>
	/// <remarks>
	/// <para>
	/// MasterPage cascades a <see cref="MasterPageContext"/> to all descendants so that
	/// <see cref="Content"/> controls can register named fragments and the host can
	/// re-render like a lightweight layout/section shim. <see cref="ContentPlaceHolder"/>
	/// then simply reads its named slot from the shared context.
	/// </para>
	/// <para>
	/// For Blazor layout scenarios (migrated master pages used with <c>@layout</c>),
	/// inherit <see cref="MasterPageLayoutBase"/> in the layout component instead.
	/// </para>
	/// <para>
	/// Original Microsoft documentation:
	/// https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.masterpage
	/// </para>
	/// </remarks>
	public partial class MasterPage : MasterPageBase
	{
		/// <summary>
		/// The content of the master page template, which typically contains layout
		/// structure and <see cref="ContentPlaceHolder"/> controls.
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Optional head content wrapped in a <c>HeadContent</c> component, bridging
		/// Web Forms' <c>&lt;head runat="server"&gt;</c> to Blazor's <c>HeadOutlet</c>.
		/// </summary>
		[Parameter]
		public RenderFragment Head { get; set; }

		/// <summary>
		/// Shared section registry consumed by <see cref="ContentPlaceHolder"/> and
		/// populated by <see cref="Content"/>.
		/// </summary>
		internal MasterPageContext Context { get; } = new();

		/// <summary>
		/// Read-only view of the content sections that have been registered by
		/// <see cref="Content"/> controls. Keyed by <c>ContentPlaceHolderID</c>.
		/// </summary>
		internal IReadOnlyDictionary<string, RenderFragment> ContentSections =>
			_contentSectionsView ??= new MasterPageContentSectionsView(Context);

		private MasterPageContentSectionsView _contentSectionsView;
		private bool _renderQueued;

		/// <summary>
		/// Gets the content registered for <paramref name="placeHolderId"/>, or <c>null</c>.
		/// </summary>
		internal RenderFragment GetContentForPlaceHolder(string placeHolderId) =>
			Context.GetContent(placeHolderId);

		/// <summary>
		/// Called by <see cref="ContentPlaceHolder"/> controls to announce their presence.
		/// Retained for backward compatibility.
		/// </summary>
		[Obsolete("ContentPlaceHolder controls now resolve content directly from MasterPageContext.")]
		internal void RegisterContentPlaceHolder(ContentPlaceHolder placeholder) { }

		/// <inheritdoc />
		protected override void OnInitialized()
		{
			base.OnInitialized();
			Context.SectionsChanged += OnSectionsChanged;
		}

		private void OnSectionsChanged()
		{
			if (_renderQueued)
			{
				return;
			}

			_renderQueued = true;
			_ = InvokeAsync(() =>
			{
				_renderQueued = false;
				StateHasChanged();
			});
		}

		/// <inheritdoc />
		protected override async ValueTask Dispose(bool disposing)
		{
			if (disposing)
			{
				Context.SectionsChanged -= OnSectionsChanged;
				Context.Dispose();
			}

			await base.Dispose(disposing);
		}
	}

	/// <summary>
	/// Read-only dictionary view over a <see cref="MasterPageContext"/> for backward
	/// compatibility with code that accesses <see cref="MasterPage.ContentSections"/>.
	/// </summary>
	internal sealed class MasterPageContentSectionsView : IReadOnlyDictionary<string, RenderFragment>
	{
		private readonly MasterPageContext _ctx;

		public MasterPageContentSectionsView(MasterPageContext ctx) => _ctx = ctx;

		public RenderFragment this[string key] => _ctx.GetContent(key) ?? throw new KeyNotFoundException(key);
		public IEnumerable<string> Keys => _ctx.RegisteredIds;
		public IEnumerable<RenderFragment> Values
		{
			get
			{
				foreach (var id in _ctx.RegisteredIds)
				{
					yield return _ctx.GetContent(id);
				}
			}
		}

		public int Count => System.Linq.Enumerable.Count(_ctx.RegisteredIds);
		public bool ContainsKey(string key) => _ctx.HasContent(key);

		public bool TryGetValue(string key, out RenderFragment value)
		{
			value = _ctx.GetContent(key);
			return value != null;
		}

		public IEnumerator<KeyValuePair<string, RenderFragment>> GetEnumerator()
		{
			foreach (var id in _ctx.RegisteredIds)
			{
				yield return new KeyValuePair<string, RenderFragment>(id, _ctx.GetContent(id));
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <summary>
	/// Base class for the <see cref="MasterPage"/> component. Carries the shared
	/// obsolete Web Forms properties that migration tooling may emit.
	/// </summary>
	public abstract class MasterPageBase : BaseWebFormsComponent
	{
		/// <summary>
		/// The page title. In Blazor use the <c>PageTitle</c> component instead.
		/// </summary>
		[Parameter, Obsolete("Use PageTitle component or set Title via IPageService instead.")]
		public string Title { get; set; }

		/// <summary>
		/// Path to a parent master page (nested master pages).
		/// In Blazor, use the <c>@layout</c> directive on the layout component instead.
		/// </summary>
		[Parameter, Obsolete("Use the @layout directive for nested layouts in Blazor.")]
		public string MasterPageFile { get; set; }
	}

	/// <summary>
	/// Abstract base class for Blazor layout components that emulate ASP.NET Web Forms
	/// master pages. Inherit this class in your migrated <c>.razor</c> layout files
	/// (those that use <c>@layout</c> from child pages) when you want to preserve named
	/// <see cref="ContentPlaceHolder"/> / <see cref="Content"/> slot relationships.
	/// </summary>
	public abstract class MasterPageLayoutBase : LayoutComponentBase, IAsyncDisposable
	{
		private static readonly FieldInfo s_renderFragmentField =
			typeof(ComponentBase).GetField("_renderFragment", BindingFlags.NonPublic | BindingFlags.Instance)
			?? throw new InvalidOperationException(
				"ComponentBase._renderFragment field not found. " +
				"This Blazor version may not be compatible with MasterPageLayoutBase.");

		private readonly RenderFragment _baseRenderFragment;
		private bool _renderQueued;

		/// <summary>
		/// The shared context that coordinates ContentPlaceHolder / Content communication.
		/// Automatically cascaded to all descendants by this base class.
		/// </summary>
		protected MasterPageContext Context { get; } = new();

		/// <summary>Initialises the context cascade wrapper.</summary>
		protected MasterPageLayoutBase()
		{
			_baseRenderFragment = (RenderFragment)s_renderFragmentField.GetValue(this)!;
			s_renderFragmentField.SetValue(this, (RenderFragment)ContextWrappedRenderTree);
			Context.SectionsChanged += OnSectionsChanged;

			void ContextWrappedRenderTree(RenderTreeBuilder builder)
			{
				builder.OpenComponent<CascadingValue<MasterPageContext>>(0);
				builder.AddAttribute(1, nameof(CascadingValue<object>.Value), Context);
				builder.AddAttribute(2, nameof(CascadingValue<object>.IsFixed), false);
				builder.AddAttribute(3, nameof(CascadingValue<object>.ChildContent), _baseRenderFragment);
				builder.CloseComponent();
			}
		}

		private void OnSectionsChanged()
		{
			if (_renderQueued)
			{
				return;
			}

			_renderQueued = true;
			_ = InvokeAsync(() =>
			{
				_renderQueued = false;
				StateHasChanged();
			});
		}

		/// <inheritdoc />
		public ValueTask DisposeAsync()
		{
			Context.SectionsChanged -= OnSectionsChanged;
			Context.Dispose();
			return ValueTask.CompletedTask;
		}
	}
}
