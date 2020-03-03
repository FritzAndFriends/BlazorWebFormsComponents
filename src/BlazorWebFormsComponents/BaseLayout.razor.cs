using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class BaseLayout : LayoutComponentBase {


		private readonly Queue<Action> _JavaScriptQueue = new Queue<Action>();

		[Inject]
		public IJSRuntime JsInterop { get; set; }

		/// <summary>
		/// Get or set the HTML page title  
		/// </summary> 
		public string Title
		{
			get { return JsInterop.InvokeAsync<string>(JavaScriptConstants.getTitle, new object[] { }).GetAwaiter().GetResult(); }
			set { JsInterop.InvokeVoidAsync(JavaScriptConstants.setTitle, new string[] { value }); }
		}

		public string MyProperty { get; set; } = "Foo for you!";

		private bool _Rendered = false;



		private T ExecuteJavaScript<T>(string method, object[] parameters)
		{

			if (_Rendered)
			{
				return JsInterop.InvokeAsync<T>(method, parameters).GetAwaiter().GetResult();
			}

			_JavaScriptQueue.Enqueue(() => ExecuteJavaScript<T>(method, parameters));
			return default(T);

		}

		private void ExecuteJavaScript(string method, object[] parameters)
		{

			if (_Rendered)
			{
				JsInterop.InvokeVoidAsync(method, parameters).GetAwaiter().GetResult();
				return;
			}

			_JavaScriptQueue.Enqueue(() => ExecuteJavaScript(method, parameters));
			return;

		}

		protected override void OnAfterRender(bool firstRender)
		{

			base.OnAfterRender(firstRender);

			while (_JavaScriptQueue.Any())
			{

				_JavaScriptQueue.Dequeue()();

			}

			_Rendered = true;

		}

	}

}
