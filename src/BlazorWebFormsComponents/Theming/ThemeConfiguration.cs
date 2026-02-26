using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.Theming
{
	/// <summary>
	/// Holds a collection of <see cref="ControlSkin"/> entries keyed by control type name.
	/// Each control type can have one default skin (empty SkinID) and any number of named
	/// skins selected via SkinID.
	/// </summary>
	public class ThemeConfiguration
	{
		private const string DefaultSkinKey = "";

		private readonly Dictionary<string, Dictionary<string, ControlSkin>> _skins
			= new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Registers a skin for a given control type.
		/// </summary>
		/// <param name="controlTypeName">The simple type name of the control (e.g. "Button", "GridView").</param>
		/// <param name="skin">The skin to register.</param>
		/// <param name="skinId">
		/// Optional SkinID for a named skin. Pass null or empty string to register the default skin.
		/// </param>
		public void AddSkin(string controlTypeName, ControlSkin skin, string skinId = null)
		{
			if (string.IsNullOrEmpty(controlTypeName))
				throw new ArgumentException("Control type name cannot be null or empty.", nameof(controlTypeName));

			if (skin is null)
				throw new ArgumentNullException(nameof(skin));

			var key = skinId ?? DefaultSkinKey;

			if (!_skins.TryGetValue(controlTypeName, out var skinMap))
			{
				skinMap = new Dictionary<string, ControlSkin>(StringComparer.OrdinalIgnoreCase);
				_skins[controlTypeName] = skinMap;
			}

			skinMap[key] = skin;
		}

		/// <summary>
		/// Retrieves a skin for a given control type and optional SkinID.
		/// Returns null if no matching skin is found (caller should log a warning
		/// for missing named skins per project convention).
		/// </summary>
		/// <param name="controlTypeName">The simple type name of the control.</param>
		/// <param name="skinId">
		/// Optional SkinID. Pass null or empty string to retrieve the default skin.
		/// </param>
		/// <returns>The matching <see cref="ControlSkin"/>, or null if not found.</returns>
		public ControlSkin GetSkin(string controlTypeName, string skinId = null)
		{
			if (string.IsNullOrEmpty(controlTypeName))
				return null;

			if (!_skins.TryGetValue(controlTypeName, out var skinMap))
				return null;

			var key = skinId ?? DefaultSkinKey;

			skinMap.TryGetValue(key, out var skin);
			return skin;
		}

		/// <summary>
		/// Returns true if at least one skin is registered for the given control type.
		/// </summary>
		public bool HasSkins(string controlTypeName)
		{
			return !string.IsNullOrEmpty(controlTypeName)
				&& _skins.ContainsKey(controlTypeName);
		}
	}
}
