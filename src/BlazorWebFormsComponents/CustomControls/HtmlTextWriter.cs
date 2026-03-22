using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Provides a Blazor-compatible implementation of the Web Forms HtmlTextWriter class
	/// for rendering custom controls. This class buffers HTML output that can be converted
	/// to a RenderFragment for use in Blazor components.
	/// </summary>
	public class HtmlTextWriter : IDisposable
	{
		private readonly StringBuilder _builder;
		private readonly Stack<string> _tagStack;
		private readonly Stack<Dictionary<string, string>> _attributeStack;
		private Dictionary<string, string> _pendingAttributes;
		private bool _tagOpen;

		/// <summary>
		/// Initializes a new instance of the HtmlTextWriter class.
		/// </summary>
		public HtmlTextWriter()
		{
			_builder = new StringBuilder();
			_tagStack = new Stack<string>();
			_attributeStack = new Stack<Dictionary<string, string>>();
			_pendingAttributes = new Dictionary<string, string>();
			_tagOpen = false;
		}

		/// <summary>
		/// Gets the rendered HTML content as a string.
		/// </summary>
		public string GetHtml() => _builder.ToString();

		/// <summary>
		/// Writes the specified string to the output.
		/// </summary>
		public void Write(string text)
		{
			ClosePendingTag();
			_builder.Append(text);
		}

		/// <summary>
		/// Writes the specified string followed by a line terminator to the output.
		/// </summary>
		public void WriteLine(string text)
		{
			ClosePendingTag();
			_builder.AppendLine(text);
		}

		/// <summary>
		/// Writes a line terminator to the output.
		/// </summary>
		public void WriteLine()
		{
			ClosePendingTag();
			_builder.AppendLine();
		}

		/// <summary>
		/// Adds the specified attribute and value to the next opening tag.
		/// </summary>
		public void AddAttribute(string name, string value)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			// Special handling for class attribute - concatenate instead of replace
			if (name.Equals("class", StringComparison.OrdinalIgnoreCase)
				&& _pendingAttributes.TryGetValue("class", out var existingClass)
				&& !string.IsNullOrEmpty(existingClass)
				&& !string.IsNullOrEmpty(value))
			{
				_pendingAttributes["class"] = $"{existingClass} {value}";
				return;
			}

			_pendingAttributes[name] = value ?? string.Empty;
		}

		/// <summary>
		/// Adds the specified attribute and value to the next opening tag.
		/// </summary>
		public void AddAttribute(HtmlTextWriterAttribute attribute, string value)
		{
			AddAttribute(GetAttributeName(attribute), value);
		}

		/// <summary>
		/// Adds a style attribute to the next opening tag.
		/// </summary>
		public void AddStyleAttribute(string name, string value)
		{
			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
				return;

			if (_pendingAttributes.TryGetValue("style", out var existingStyle))
			{
				// Ensure existing style ends with semicolon before concatenating
				var trimmedStyle = existingStyle.TrimEnd();
				if (!trimmedStyle.EndsWith(";"))
				{
					trimmedStyle += ";";
				}
				_pendingAttributes["style"] = $"{trimmedStyle} {name}: {value};";
			}
			else
			{
				// Ensure new style ends with semicolon for consistency
				_pendingAttributes["style"] = $"{name}: {value};";
			}
		}

		/// <summary>
		/// Adds a style attribute to the next opening tag.
		/// </summary>
		public void AddStyleAttribute(HtmlTextWriterStyle style, string value)
		{
			AddStyleAttribute(GetStyleName(style), value);
		}

		/// <summary>
		/// Writes the opening tag of the specified HTML element to the output.
		/// </summary>
		public void RenderBeginTag(string tagName)
		{
			if (string.IsNullOrEmpty(tagName))
				throw new ArgumentNullException(nameof(tagName));

			ClosePendingTag();

			_tagStack.Push(tagName);
			_attributeStack.Push(_pendingAttributes);
			_pendingAttributes = new Dictionary<string, string>();

			_builder.Append($"<{tagName}");
			_tagOpen = true;

			var attributes = _attributeStack.Peek();
			if (attributes.Count > 0)
			{
				foreach (var attr in attributes)
				{
					_builder.Append($" {attr.Key}=\"{EncodeAttributeValue(attr.Value)}\"");
				}
			}
		}

		/// <summary>
		/// Writes the opening tag of the specified HTML element to the output.
		/// </summary>
		public void RenderBeginTag(HtmlTextWriterTag tag)
		{
			RenderBeginTag(GetTagName(tag));
		}

		/// <summary>
		/// Writes the closing tag of the current HTML element to the output.
		/// </summary>
		public void RenderEndTag()
		{
			if (_tagStack.Count == 0)
				throw new InvalidOperationException("No tag to close.");

			ClosePendingTag();

			var tagName = _tagStack.Pop();
			_attributeStack.Pop();
			_builder.Append($"</{tagName}>");
		}

		/// <summary>
		/// Closes any pending opening tag.
		/// </summary>
		private void ClosePendingTag()
		{
			if (_tagOpen)
			{
				_builder.Append(">");
				_tagOpen = false;
			}
		}

		/// <summary>
		/// Encodes the specified attribute value for HTML output.
		/// </summary>
		private string EncodeAttributeValue(string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return value
				.Replace("&", "&amp;")
				.Replace("\"", "&quot;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}

		/// <summary>
		/// Gets the HTML tag name for the specified enum value.
		/// </summary>
		private string GetTagName(HtmlTextWriterTag tag)
		{
			return tag switch
			{
				HtmlTextWriterTag.A => "a",
				HtmlTextWriterTag.Button => "button",
				HtmlTextWriterTag.Div => "div",
				HtmlTextWriterTag.Span => "span",
				HtmlTextWriterTag.Input => "input",
				HtmlTextWriterTag.Label => "label",
				HtmlTextWriterTag.P => "p",
				HtmlTextWriterTag.Table => "table",
				HtmlTextWriterTag.Tr => "tr",
				HtmlTextWriterTag.Td => "td",
				HtmlTextWriterTag.Th => "th",
				HtmlTextWriterTag.Tbody => "tbody",
				HtmlTextWriterTag.Thead => "thead",
				HtmlTextWriterTag.Ul => "ul",
				HtmlTextWriterTag.Li => "li",
				HtmlTextWriterTag.Select => "select",
				HtmlTextWriterTag.Option => "option",
				HtmlTextWriterTag.Img => "img",
				HtmlTextWriterTag.H1 => "h1",
				HtmlTextWriterTag.H2 => "h2",
				HtmlTextWriterTag.H3 => "h3",
				HtmlTextWriterTag.H4 => "h4",
				HtmlTextWriterTag.H5 => "h5",
				HtmlTextWriterTag.H6 => "h6",
				HtmlTextWriterTag.Form => "form",
				HtmlTextWriterTag.Abbr => "abbr",
				HtmlTextWriterTag.Address => "address",
				HtmlTextWriterTag.Area => "area",
				HtmlTextWriterTag.Article => "article",
				HtmlTextWriterTag.Aside => "aside",
				HtmlTextWriterTag.Audio => "audio",
				HtmlTextWriterTag.Blockquote => "blockquote",
				HtmlTextWriterTag.Br => "br",
				HtmlTextWriterTag.Canvas => "canvas",
				HtmlTextWriterTag.Caption => "caption",
				HtmlTextWriterTag.Cite => "cite",
				HtmlTextWriterTag.Code => "code",
				HtmlTextWriterTag.Col => "col",
				HtmlTextWriterTag.Colgroup => "colgroup",
				HtmlTextWriterTag.Datalist => "datalist",
				HtmlTextWriterTag.Dd => "dd",
				HtmlTextWriterTag.Details => "details",
				HtmlTextWriterTag.Dialog => "dialog",
				HtmlTextWriterTag.Dl => "dl",
				HtmlTextWriterTag.Dt => "dt",
				HtmlTextWriterTag.Em => "em",
				HtmlTextWriterTag.Fieldset => "fieldset",
				HtmlTextWriterTag.Figcaption => "figcaption",
				HtmlTextWriterTag.Figure => "figure",
				HtmlTextWriterTag.Footer => "footer",
				HtmlTextWriterTag.Header => "header",
				HtmlTextWriterTag.Hr => "hr",
				HtmlTextWriterTag.Iframe => "iframe",
				HtmlTextWriterTag.Legend => "legend",
				HtmlTextWriterTag.Main => "main",
				HtmlTextWriterTag.Map => "map",
				HtmlTextWriterTag.Mark => "mark",
				HtmlTextWriterTag.Meter => "meter",
				HtmlTextWriterTag.Nav => "nav",
				HtmlTextWriterTag.Ol => "ol",
				HtmlTextWriterTag.Output => "output",
				HtmlTextWriterTag.Pre => "pre",
				HtmlTextWriterTag.Progress => "progress",
				HtmlTextWriterTag.Rp => "rp",
				HtmlTextWriterTag.Rt => "rt",
				HtmlTextWriterTag.Ruby => "ruby",
				HtmlTextWriterTag.Samp => "samp",
				HtmlTextWriterTag.Section => "section",
				HtmlTextWriterTag.Small => "small",
				HtmlTextWriterTag.Strong => "strong",
				HtmlTextWriterTag.Sub => "sub",
				HtmlTextWriterTag.Summary => "summary",
				HtmlTextWriterTag.Sup => "sup",
				HtmlTextWriterTag.Template => "template",
				HtmlTextWriterTag.Textarea => "textarea",
				HtmlTextWriterTag.Tfoot => "tfoot",
				HtmlTextWriterTag.Time => "time",
				HtmlTextWriterTag.Var => "var",
				HtmlTextWriterTag.Video => "video",
				HtmlTextWriterTag.Wbr => "wbr",
				_ => tag.ToString().ToLowerInvariant()
			};
		}

		/// <summary>
		/// Gets the HTML attribute name for the specified enum value.
		/// </summary>
		private string GetAttributeName(HtmlTextWriterAttribute attribute)
		{
			return attribute switch
			{
				HtmlTextWriterAttribute.Id => "id",
				HtmlTextWriterAttribute.Class => "class",
				HtmlTextWriterAttribute.Style => "style",
				HtmlTextWriterAttribute.Href => "href",
				HtmlTextWriterAttribute.Src => "src",
				HtmlTextWriterAttribute.Alt => "alt",
				HtmlTextWriterAttribute.Name => "name",
				HtmlTextWriterAttribute.Type => "type",
				HtmlTextWriterAttribute.Value => "value",
				HtmlTextWriterAttribute.Title => "title",
				HtmlTextWriterAttribute.Width => "width",
				HtmlTextWriterAttribute.Height => "height",
				HtmlTextWriterAttribute.Disabled => "disabled",
				HtmlTextWriterAttribute.Readonly => "readonly",
				HtmlTextWriterAttribute.Action => "action",
				HtmlTextWriterAttribute.AriaControls => "aria-controls",
				HtmlTextWriterAttribute.AriaDescribedby => "aria-describedby",
				HtmlTextWriterAttribute.AriaDisabled => "aria-disabled",
				HtmlTextWriterAttribute.AriaExpanded => "aria-expanded",
				HtmlTextWriterAttribute.AriaHidden => "aria-hidden",
				HtmlTextWriterAttribute.AriaLabel => "aria-label",
				HtmlTextWriterAttribute.AriaLabelledby => "aria-labelledby",
				HtmlTextWriterAttribute.AriaLive => "aria-live",
				HtmlTextWriterAttribute.AriaSelected => "aria-selected",
				HtmlTextWriterAttribute.Autocomplete => "autocomplete",
				HtmlTextWriterAttribute.Autofocus => "autofocus",
				HtmlTextWriterAttribute.Checked => "checked",
				HtmlTextWriterAttribute.Colspan => "colspan",
				HtmlTextWriterAttribute.Contenteditable => "contenteditable",
				HtmlTextWriterAttribute.Dir => "dir",
				HtmlTextWriterAttribute.Download => "download",
				HtmlTextWriterAttribute.Draggable => "draggable",
				HtmlTextWriterAttribute.Enctype => "enctype",
				HtmlTextWriterAttribute.For => "for",
				HtmlTextWriterAttribute.Headers => "headers",
				HtmlTextWriterAttribute.Hidden => "hidden",
				HtmlTextWriterAttribute.Lang => "lang",
				HtmlTextWriterAttribute.Max => "max",
				HtmlTextWriterAttribute.Maxlength => "maxlength",
				HtmlTextWriterAttribute.Method => "method",
				HtmlTextWriterAttribute.Min => "min",
				HtmlTextWriterAttribute.Minlength => "minlength",
				HtmlTextWriterAttribute.Multiple => "multiple",
				HtmlTextWriterAttribute.Open => "open",
				HtmlTextWriterAttribute.Pattern => "pattern",
				HtmlTextWriterAttribute.Placeholder => "placeholder",
				HtmlTextWriterAttribute.Rel => "rel",
				HtmlTextWriterAttribute.Required => "required",
				HtmlTextWriterAttribute.Role => "role",
				HtmlTextWriterAttribute.Rowspan => "rowspan",
				HtmlTextWriterAttribute.Scope => "scope",
				HtmlTextWriterAttribute.Selected => "selected",
				HtmlTextWriterAttribute.Step => "step",
				HtmlTextWriterAttribute.Tabindex => "tabindex",
				HtmlTextWriterAttribute.Target => "target",
				_ => attribute.ToString().ToLowerInvariant()
			};
		}

		/// <summary>
		/// Gets the CSS style name for the specified enum value.
		/// </summary>
		private string GetStyleName(HtmlTextWriterStyle style)
		{
			return style switch
			{
				HtmlTextWriterStyle.BackgroundColor => "background-color",
				HtmlTextWriterStyle.Color => "color",
				HtmlTextWriterStyle.FontFamily => "font-family",
				HtmlTextWriterStyle.FontSize => "font-size",
				HtmlTextWriterStyle.FontWeight => "font-weight",
				HtmlTextWriterStyle.FontStyle => "font-style",
				HtmlTextWriterStyle.Height => "height",
				HtmlTextWriterStyle.Width => "width",
				HtmlTextWriterStyle.BorderColor => "border-color",
				HtmlTextWriterStyle.BorderStyle => "border-style",
				HtmlTextWriterStyle.BorderWidth => "border-width",
				HtmlTextWriterStyle.Margin => "margin",
				HtmlTextWriterStyle.Padding => "padding",
				HtmlTextWriterStyle.TextAlign => "text-align",
				HtmlTextWriterStyle.Display => "display",
				HtmlTextWriterStyle.AlignContent => "align-content",
				HtmlTextWriterStyle.AlignItems => "align-items",
				HtmlTextWriterStyle.AlignSelf => "align-self",
				HtmlTextWriterStyle.Animation => "animation",
				HtmlTextWriterStyle.BackgroundImage => "background-image",
				HtmlTextWriterStyle.BackgroundPosition => "background-position",
				HtmlTextWriterStyle.BackgroundRepeat => "background-repeat",
				HtmlTextWriterStyle.BackgroundSize => "background-size",
				HtmlTextWriterStyle.BorderRadius => "border-radius",
				HtmlTextWriterStyle.Bottom => "bottom",
				HtmlTextWriterStyle.BoxShadow => "box-shadow",
				HtmlTextWriterStyle.BoxSizing => "box-sizing",
				HtmlTextWriterStyle.Clear => "clear",
				HtmlTextWriterStyle.Cursor => "cursor",
				HtmlTextWriterStyle.FlexBasis => "flex-basis",
				HtmlTextWriterStyle.FlexDirection => "flex-direction",
				HtmlTextWriterStyle.FlexGrow => "flex-grow",
				HtmlTextWriterStyle.FlexShrink => "flex-shrink",
				HtmlTextWriterStyle.FlexWrap => "flex-wrap",
				HtmlTextWriterStyle.Float => "float",
				HtmlTextWriterStyle.Gap => "gap",
				HtmlTextWriterStyle.GridColumn => "grid-column",
				HtmlTextWriterStyle.GridGap => "grid-gap",
				HtmlTextWriterStyle.GridRow => "grid-row",
				HtmlTextWriterStyle.GridTemplateColumns => "grid-template-columns",
				HtmlTextWriterStyle.GridTemplateRows => "grid-template-rows",
				HtmlTextWriterStyle.JustifyContent => "justify-content",
				HtmlTextWriterStyle.Left => "left",
				HtmlTextWriterStyle.LetterSpacing => "letter-spacing",
				HtmlTextWriterStyle.LineHeight => "line-height",
				HtmlTextWriterStyle.ListStylePosition => "list-style-position",
				HtmlTextWriterStyle.ListStyleType => "list-style-type",
				HtmlTextWriterStyle.MarginBottom => "margin-bottom",
				HtmlTextWriterStyle.MarginLeft => "margin-left",
				HtmlTextWriterStyle.MarginRight => "margin-right",
				HtmlTextWriterStyle.MarginTop => "margin-top",
				HtmlTextWriterStyle.MaxHeight => "max-height",
				HtmlTextWriterStyle.MaxWidth => "max-width",
				HtmlTextWriterStyle.MinHeight => "min-height",
				HtmlTextWriterStyle.MinWidth => "min-width",
				HtmlTextWriterStyle.Opacity => "opacity",
				HtmlTextWriterStyle.OutlineColor => "outline-color",
				HtmlTextWriterStyle.OutlineStyle => "outline-style",
				HtmlTextWriterStyle.OutlineWidth => "outline-width",
				HtmlTextWriterStyle.Overflow => "overflow",
				HtmlTextWriterStyle.PaddingBottom => "padding-bottom",
				HtmlTextWriterStyle.PaddingLeft => "padding-left",
				HtmlTextWriterStyle.PaddingRight => "padding-right",
				HtmlTextWriterStyle.PaddingTop => "padding-top",
				HtmlTextWriterStyle.Position => "position",
				HtmlTextWriterStyle.Right => "right",
				HtmlTextWriterStyle.TextDecoration => "text-decoration",
				HtmlTextWriterStyle.TextOverflow => "text-overflow",
				HtmlTextWriterStyle.TextTransform => "text-transform",
				HtmlTextWriterStyle.Top => "top",
				HtmlTextWriterStyle.Transform => "transform",
				HtmlTextWriterStyle.Transition => "transition",
				HtmlTextWriterStyle.VerticalAlign => "vertical-align",
				HtmlTextWriterStyle.Visibility => "visibility",
				HtmlTextWriterStyle.WhiteSpace => "white-space",
				HtmlTextWriterStyle.WordWrap => "word-wrap",
				HtmlTextWriterStyle.ZIndex => "z-index",
				_ => style.ToString().ToLowerInvariant()
			};
		}

		/// <summary>
		/// Disposes resources used by the HtmlTextWriter.
		/// </summary>
		public void Dispose()
		{
			// Clean up any remaining tags
			ClosePendingTag();
		}
	}

	/// <summary>
	/// Specifies common HTML tags for use with HtmlTextWriter.
	/// </summary>
	public enum HtmlTextWriterTag
	{
		A,
		Button,
		Div,
		Span,
		Input,
		Label,
		P,
		Table,
		Tr,
		Td,
		Th,
		Tbody,
		Thead,
		Ul,
		Li,
		Select,
		Option,
		Img,
		H1,
		H2,
		H3,
		H4,
		H5,
		H6,
		Form,
		Nav,
		Section,
		Article,
		Header,
		Footer,
		Main,
		Figure,
		Figcaption,
		Details,
		Summary,
		Dialog,
		Template,
		Fieldset,
		Legend,
		Textarea,
		Br,
		Hr,
		Em,
		Strong,
		Small,
		Code,
		Pre,
		Blockquote,
		Ol,
		Dl,
		Dt,
		Dd,
		Iframe,
		Video,
		Audio,
		Canvas,
		Progress,
		Meter,
		Abbr,
		Address,
		Aside,
		Caption,
		Cite,
		Col,
		Colgroup,
		Datalist,
		Map,
		Area,
		Mark,
		Output,
		Ruby,
		Rt,
		Rp,
		Samp,
		Sub,
		Sup,
		Time,
		Var,
		Wbr,
		Tfoot
	}

	/// <summary>
	/// Specifies common HTML attributes for use with HtmlTextWriter.
	/// </summary>
	public enum HtmlTextWriterAttribute
	{
		Id,
		Class,
		Style,
		Href,
		Src,
		Alt,
		Name,
		Type,
		Value,
		Title,
		Width,
		Height,
		Disabled,
		Readonly,
		Placeholder,
		Required,
		Autofocus,
		Pattern,
		Min,
		Max,
		Step,
		Maxlength,
		Minlength,
		Multiple,
		Autocomplete,
		Target,
		Rel,
		Download,
		Action,
		Method,
		Enctype,
		Colspan,
		Rowspan,
		Scope,
		Headers,
		For,
		Checked,
		Selected,
		Open,
		Role,
		Tabindex,
		Contenteditable,
		Draggable,
		Hidden,
		Lang,
		Dir,
		AriaLabel,
		AriaHidden,
		AriaExpanded,
		AriaDescribedby,
		AriaLabelledby,
		AriaLive,
		AriaControls,
		AriaSelected,
		AriaDisabled
	}

	/// <summary>
	/// Specifies common CSS styles for use with HtmlTextWriter.
	/// </summary>
	public enum HtmlTextWriterStyle
	{
		BackgroundColor,
		Color,
		FontFamily,
		FontSize,
		FontWeight,
		FontStyle,
		Height,
		Width,
		BorderColor,
		BorderStyle,
		BorderWidth,
		Margin,
		Padding,
		TextAlign,
		Display,
		FlexDirection,
		JustifyContent,
		AlignItems,
		AlignContent,
		AlignSelf,
		FlexWrap,
		FlexGrow,
		FlexShrink,
		FlexBasis,
		Gap,
		GridTemplateColumns,
		GridTemplateRows,
		GridColumn,
		GridRow,
		GridGap,
		Transform,
		Transition,
		Animation,
		Opacity,
		BoxShadow,
		BorderRadius,
		Position,
		Top,
		Right,
		Bottom,
		Left,
		ZIndex,
		Overflow,
		Float,
		Clear,
		Cursor,
		Visibility,
		TextDecoration,
		TextTransform,
		TextOverflow,
		WhiteSpace,
		WordWrap,
		LetterSpacing,
		LineHeight,
		VerticalAlign,
		MinWidth,
		MaxWidth,
		MinHeight,
		MaxHeight,
		BoxSizing,
		MarginTop,
		MarginRight,
		MarginBottom,
		MarginLeft,
		PaddingTop,
		PaddingRight,
		PaddingBottom,
		PaddingLeft,
		BackgroundImage,
		BackgroundPosition,
		BackgroundRepeat,
		BackgroundSize,
		OutlineColor,
		OutlineStyle,
		OutlineWidth,
		ListStyleType,
		ListStylePosition
	}
}
