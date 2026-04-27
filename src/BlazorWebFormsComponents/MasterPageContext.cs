using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Shared context object cascaded by <see cref="MasterPage"/> and
	/// <see cref="MasterPageLayoutBase"/> to coordinate named content-slot
	/// communication between <see cref="ContentPlaceHolder"/> controls (which
	/// consume slots) and <see cref="Content"/> controls (which populate slots).
	/// </summary>
	/// <remarks>
	/// <para>
	/// ContentPlaceHolder controls subscribe to their named slot via
	/// <see cref="Subscribe"/>. When a Content control later calls
	/// <see cref="SetContent"/>, the subscriber is notified immediately so it
	/// can call <c>StateHasChanged</c> and re-render with the injected fragment.
	/// </para>
	/// <para>
	/// This solves the classic Web Forms migration timing problem: in a Blazor
	/// layout the layout template (containing ContentPlaceHolder) renders
	/// <em>before</em> the child page content (containing Content), so a
	/// simple one-shot dictionary read in <c>OnInitialized</c> always sees an
	/// empty collection. The subscription model lets the placeholder update
	/// itself the moment its content arrives, regardless of render order.
	/// </para>
	/// </remarks>
	public sealed class MasterPageContext : IDisposable
	{
		private readonly Dictionary<string, RenderFragment?> _sections =
			new(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, Action> _callbacks =
			new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Register or update the content fragment for a named placeholder slot.
		/// If the fragment reference has changed, any subscribed
		/// <see cref="ContentPlaceHolder"/> with the matching ID is notified.
		/// </summary>
		public void SetContent(string placeholderId, RenderFragment? content)
		{
			if (string.IsNullOrEmpty(placeholderId)) return;

			// Only notify if the fragment reference actually changed to avoid
			// unnecessary re-renders on repeated OnParametersSet calls.
			var changed = !_sections.TryGetValue(placeholderId, out var existing)
						  || !ReferenceEquals(existing, content);

			_sections[placeholderId] = content;

			if (changed && _callbacks.TryGetValue(placeholderId, out var callback))
				callback();
		}

		/// <summary>Returns the registered content for <paramref name="placeholderId"/>, or <c>null</c>.</summary>
		public RenderFragment? GetContent(string placeholderId)
		{
			if (string.IsNullOrEmpty(placeholderId)) return null;
			return _sections.TryGetValue(placeholderId, out var content) ? content : null;
		}

		/// <summary>
		/// Returns <c>true</c> if any <see cref="Content"/> control has registered
		/// a fragment for <paramref name="placeholderId"/>.
		/// </summary>
		public bool HasContent(string placeholderId) =>
			!string.IsNullOrEmpty(placeholderId) && _sections.ContainsKey(placeholderId);

		/// <summary>
		/// Returns the IDs of all registered content slots (useful for diagnostics).
		/// </summary>
		public IEnumerable<string> RegisteredIds => _sections.Keys;

		/// <summary>
		/// Subscribe <paramref name="onChanged"/> so that it is invoked immediately
		/// whenever <see cref="SetContent"/> records a new fragment for
		/// <paramref name="placeholderId"/>. Only one subscriber per ID is supported;
		/// a second call for the same ID replaces the previous subscriber.
		/// </summary>
		internal void Subscribe(string placeholderId, Action onChanged)
		{
			if (!string.IsNullOrEmpty(placeholderId))
				_callbacks[placeholderId] = onChanged;
		}

		/// <summary>Unsubscribe the <see cref="ContentPlaceHolder"/> for <paramref name="placeholderId"/>.</summary>
		internal void Unsubscribe(string placeholderId)
		{
			if (!string.IsNullOrEmpty(placeholderId))
				_callbacks.Remove(placeholderId);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			_sections.Clear();
			_callbacks.Clear();
		}
	}
}
