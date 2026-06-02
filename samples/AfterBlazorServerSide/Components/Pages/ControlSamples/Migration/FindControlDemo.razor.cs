using BlazorWebFormsComponents;

namespace AfterBlazorServerSide.Components.Pages.ControlSamples.Migration;

public partial class FindControlDemo
{
	private string _directBoxText = "Hello from DirectBox";
	private string _directResult = "";
	private string _nestedBoxText = "Hello from NestedBox";
	private string _nestedResult = "";
	private string _chainLabelText = "Chained Label";
	private string _chainedResult = "";
	private string _caseBoxText = "Case Insensitive";
	private string _caseResult = "";

	private BlazorWebFormsComponents.Panel? _directParent;
	private BlazorWebFormsComponents.Panel? _outerPanel;
	private BlazorWebFormsComponents.Panel? _chainParent;
	private BlazorWebFormsComponents.Panel? _caseParent;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (firstRender)
		{
			// 1. Direct child lookup with cast
			if (_directParent != null)
			{
				var box = (TextBox)_directParent.FindControl("DirectBox");
				_directResult = box != null ? $"Found! Text='{box.Text}'" : "NOT FOUND";
			}

			// 2. Nested/recursive lookup
			if (_outerPanel != null)
			{
				var nested = (TextBox)_outerPanel.FindControl("NestedBox");
				_nestedResult = nested != null ? $"Found! Text='{nested.Text}'" : "NOT FOUND";
			}

			// 3. Chained lookup
			if (_chainParent != null)
			{
				var child = _chainParent.FindControl("ChainChild");
				if (child != null)
				{
					var label = (BlazorWebFormsComponents.Label)child.FindControl("ChainLabel");
					_chainedResult = label != null ? $"Found! Text='{label.Text}'" : "NOT FOUND";
				}
			}

			// 4. Case-insensitive
			if (_caseParent != null)
			{
				var caseBox = (TextBox)_caseParent.FindControl("mixedcasebox");
				_caseResult = caseBox != null ? $"Found! Text='{caseBox.Text}'" : "NOT FOUND";
			}

			StateHasChanged();
		}
	}
}
