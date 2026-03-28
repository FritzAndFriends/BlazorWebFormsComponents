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
		/// Controls how theme skins interact with explicit property values.
		/// Default is StyleSheetTheme (theme sets defaults, explicit values win).
		/// </summary>
		public ThemeMode Mode { get; set; } = ThemeMode.StyleSheetTheme;

		/// <summary>
		/// CSS files to be included when this theme is active.
		/// ThemeProvider will render &lt;link&gt; elements for each file.
		/// </summary>
		public List<string> CssFiles { get; set; }

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

		/// <summary>
		/// Fluent API: registers a default skin for a control type using a builder action.
		/// </summary>
		public ThemeConfiguration ForControl(string controlTypeName, Action<SkinBuilder> configure)
		{
			if (configure is null)
				throw new ArgumentNullException(nameof(configure));

			var builder = new SkinBuilder();
			configure(builder);
			AddSkin(controlTypeName, builder.Skin);
			return this;
		}

		/// <summary>
		/// Fluent API: registers a named skin for a control type using a builder action.
		/// </summary>
		public ThemeConfiguration ForControl(string controlTypeName, string skinId, Action<SkinBuilder> configure)
		{
			if (configure is null)
				throw new ArgumentNullException(nameof(configure));

			var builder = new SkinBuilder();
			configure(builder);
			AddSkin(controlTypeName, builder.Skin, skinId);
			return this;
		}

		/// <summary>
		/// Fluent API: sets the theme mode.
		/// </summary>
		public ThemeConfiguration WithMode(ThemeMode mode)
		{
			Mode = mode;
			return this;
		}

		/// <summary>
		/// Fluent API: adds a CSS file to the theme.
		/// </summary>
		public ThemeConfiguration WithCssFile(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentException("CSS file path cannot be null or empty.", nameof(path));

			CssFiles ??= new List<string>();
			CssFiles.Add(path);
			return this;
		}

		/// <summary>
		/// Fluent API: adds multiple CSS files to the theme.
		/// </summary>
		public ThemeConfiguration WithCssFiles(params string[] paths)
		{
			if (paths is null || paths.Length == 0)
				throw new ArgumentException("At least one CSS file path must be provided.", nameof(paths));

			foreach (var path in paths)
				WithCssFile(path);

			return this;
		}
	}
}
