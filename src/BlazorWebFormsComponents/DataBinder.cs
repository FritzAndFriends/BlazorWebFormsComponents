using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// This emulates the DataBinder from Web Forms, but is NOT recommended for long term use
	/// </summary>
	[Obsolete("🚨 Do not use the DataBinder long-term in your projects.  See LINK for documentation about how to migrate from the DataBinder 🚨")]
	public static class DataBinder
	{

		/// <summary>
		/// 🚨🚨 This does NOT return a string and is expected to be used without a predicate 🚨🚨
		/// </summary>
		/// <param name="container"></param>
		/// <param name="expression"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static RenderFragment Eval(object container, string expression, string format = "")
		{
			return Eval(expression, format);
		}

		/// <summary>
		/// 🚨🚨 This does <b>NOT</b> return a string and is expected to be used without a predicate 🚨🚨
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static RenderFragment Eval(string fieldName, string format = "")
		{
			RenderFragment fragment = builder =>
			{

				builder.OpenComponent<HelperComponents.Eval>(0);
				builder.AddAttribute(1, "PropertyName", fieldName);
				if (format != string.Empty)
				{
					builder.AddAttribute(2, "Format", format);
				}
				builder.CloseComponent();

			};

			return fragment;
		}

		/// <summary>
		/// GetDataItem is not supported with Blazor. Please use @context or @item notation to get the item.  See https://github.com/FritzAndFriends/BlazorWebFormsComponents/docs for more details
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[Obsolete("GetDataItem is not supported with Blazor", true)]
		public static object GetDataItem(object obj)
		{
			throw new NotImplementedException("GetDataItem is not supported with Blazor. Please use @context or @item notation to get the item.  See LINK for more details");
		}

		/// <summary>
		/// GetDataItem is not supported with Blazor. Please use @context or @item notation to get the item.  See LINK for more details
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="foundDataItem"></param>
		/// <returns></returns>
		[Obsolete("GetDataItem is not supported with Blazor", true)]
		public static object GetDataItem(object obj, out bool foundDataItem)
		{
			throw new NotImplementedException("GetDataItem is not supported with Blazor. Please use @context or @item notation to get the item.  See LINK for more details");
		}

		/// <summary>
		/// Gets the value of the property specified by <paramref name="propName"/> on the object specified by <paramref name="container"/>.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="propName"></param>
		/// <returns></returns>
		public static object GetPropertyValue(object container, string propName)
		{
			if (container is null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			var prop = TypeDescriptor.GetProperties(container).Find(propName, ignoreCase: true);
			if (prop == null)
			{
				throw new ArgumentException($"A property named '{propName}' could not be found on type {container.GetType().FullName}.", nameof(propName));
			}

			return prop.GetValue(container);
		}

		public static string GetPropertyValue(object container, string propName, string format)
		{
			var rawValue = GetPropertyValue(container, propName);

			return string.Format(format, rawValue);
		}
	}
}
