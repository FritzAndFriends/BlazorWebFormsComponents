using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

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
		/// 🚨🚨 This does NOT return a string and is expected to be used without a predicate 🚨🚨
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

		public static object GetDataItem(object obj)
		{
			throw new NotImplementedException();
		}

		public static object GetDataItem(object obj, out bool foundDataItem)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method acts directly on the object requested, and we're not sure why you wouldn't call `container.propName` directly
		/// </summary>
		/// <param name="container"></param>
		/// <param name="propName"></param>
		/// <returns></returns>
		public static object GetPropertyValue(object container, string propName)
		{

			var theType = container.GetType();
			var prop = theType.GetProperty(propName);

			return prop.GetValue(container);

		}

		public static string GetPropertyValue(object container, string propName, string format)
		{

			var theType = container.GetType();
			var prop = theType.GetProperty(propName);

			return string.Format(format, prop.GetValue(container));

		}

	}

}
