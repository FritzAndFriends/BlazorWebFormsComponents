using BlazorWebFormsComponents;

namespace AfterBlazorServerSide.Components.Pages.ControlSamples.Migration;

public partial class FindControlDemo
{
	private string _directBoxText = "";
	private string _directResult = "Waiting for render...";
	private string _nestedBoxText = "";
	private string _nestedResult = "Waiting for render...";
	private string _chainLabelText = "";
	private string _chainedResult = "Waiting for render...";
	private string _caseBoxText = "";
	private string _caseResult = "Waiting for render...";

	private BlazorWebFormsComponents.Panel? _directParent;
	private BlazorWebFormsComponents.Panel? _outerPanel;
	private BlazorWebFormsComponents.Panel? _chainParent;
	private BlazorWebFormsComponents.Panel? _caseParent;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (firstRender)
		{
			// 1. Direct child lookup — find it and set its Text (just like the code sample)
			if (_directParent != null)
			{
				var box = (TextBox)_directParent.FindControl("DirectBox");
				if (box != null)
				{
					box.Text = "Found it!";
					_directBoxText = box.Text;
					_directResult = "FindControl found the TextBox and set Text = \"Found it!\"";
				}
			}

			// 2. Nested/recursive lookup
			if (_outerPanel != null)
			{
				var nested = (TextBox)_outerPanel.FindControl("NestedBox");
				if (nested != null)
				{
					nested.Text = "Found deep!";
					_nestedBoxText = nested.Text;
					_nestedResult = "FindControl recursed through InnerPanel to find NestedBox";
				}
			}

			// 3. Chained lookup
			if (_chainParent != null)
			{
				var child = _chainParent.FindControl("ChainChild");
				if (child != null)
				{
					var label = (BlazorWebFormsComponents.Label)child.FindControl("ChainLabel");
					if (label != null)
					{
						label.Text = "Chained!";
						_chainLabelText = label.Text;
						_chainedResult = "Chained: parent.FindControl(\"ChainChild\").FindControl(\"ChainLabel\") succeeded";
					}
				}
			}

			// 4. Case-insensitive — note lowercase "mixedcasebox" finds ID="MixedCaseBox"
			if (_caseParent != null)
			{
				var caseBox = (TextBox)_caseParent.FindControl("mixedcasebox");
				if (caseBox != null)
				{
					caseBox.Text = "Case doesn't matter!";
					_caseBoxText = caseBox.Text;
					_caseResult = "FindControl(\"mixedcasebox\") matched ID=\"MixedCaseBox\"";
				}
			}

			StateHasChanged();
		}
	}
}
