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
	/// The context behaves like a lightweight section registry layered over Blazor
	/// layouts: <see cref="Content"/> registers a fragment for a named slot and the
	/// owning master-page host re-renders when the registry changes.
	/// </para>
	/// <para>
	/// <see cref="ContentPlaceHolder"/> remains a thin reader over that registry,
	/// which keeps the migration-facing tag names while moving the behavior closer
	/// to Blazor's normal layout/section rendering model.
	/// </para>
	/// </remarks>
	public sealed class MasterPageContext : IDisposable
	{
		private readonly Dictionary<string, RenderFragment> _sections =
			new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Raised when the section registry changes and the owning master-page host
		/// should re-render.
		/// </summary>
		public event Action SectionsChanged;

		/// <summary>
		/// Register, update, or remove the content fragment for a named placeholder slot.
		/// </summary>
		public void SetContent(string placeholderId, RenderFragment content)
		{
			if (string.IsNullOrWhiteSpace(placeholderId))
			{
				return;
			}

			var hadExisting = _sections.TryGetValue(placeholderId, out var existing);
			var changed = false;

			if (content is null)
			{
				if (hadExisting)
				{
					_sections.Remove(placeholderId);
					changed = true;
				}
			}
			else
			{
				changed = !hadExisting || !ReferenceEquals(existing, content);
				_sections[placeholderId] = content;
			}

			if (changed)
			{
				SectionsChanged?.Invoke();
			}
		}

		/// <summary>Returns the registered content for <paramref name="placeholderId"/>, or <c>null</c>.</summary>
		public RenderFragment GetContent(string placeholderId)
		{
			if (string.IsNullOrWhiteSpace(placeholderId))
			{
				return null;
			}

			return _sections.TryGetValue(placeholderId, out var content) ? content : null;
		}

		/// <summary>
		/// Returns <c>true</c> if any <see cref="Content"/> control has registered
		/// a fragment for <paramref name="placeholderId"/>.
		/// </summary>
		public bool HasContent(string placeholderId) =>
			!string.IsNullOrWhiteSpace(placeholderId) && _sections.ContainsKey(placeholderId);

		/// <summary>
		/// Returns the IDs of all registered content slots (useful for diagnostics).
		/// </summary>
		public IEnumerable<string> RegisteredIds => _sections.Keys;

		/// <inheritdoc/>
		public void Dispose()
		{
			_sections.Clear();
			SectionsChanged = null;
		}
	}
}
