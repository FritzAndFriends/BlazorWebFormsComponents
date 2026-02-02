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
				_ => throw new ArgumentException($"Unsupported tag: {tag}", nameof(tag))
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
				_ => throw new ArgumentException($"Unsupported attribute: {attribute}", nameof(attribute))
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
				_ => throw new ArgumentException($"Unsupported style: {style}", nameof(style))
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
		Form
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
		Readonly
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
		Display
	}
}
