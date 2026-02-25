using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class MultiView : BaseWebFormsComponent
	{
		public const string NextViewCommandName = "NextView";
		public const string PreviousViewCommandName = "PrevView";
		public const string SwitchViewByIDCommandName = "SwitchViewByID";
		public const string SwitchViewByIndexCommandName = "SwitchViewByIndex";

		private int _activeViewIndex = -1;

		[Parameter]
		public int ActiveViewIndex
		{
			get => _activeViewIndex;
			set
			{
				if (value < -1 || (Views.Count > 0 && value >= Views.Count))
				{
					throw new ArgumentOutOfRangeException(nameof(ActiveViewIndex),
						$"ActiveViewIndex is set to '{value}', which is out of range. Must be between -1 and {Views.Count - 1}.");
				}
				if (_activeViewIndex != value)
				{
					UpdateActiveView(_activeViewIndex, value);
					_activeViewIndex = value;
				}
			}
		}

		[Parameter]
		public EventCallback<EventArgs> OnActiveViewChanged { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		public List<View> Views { get; } = new List<View>();

		public View GetActiveView()
		{
			if (_activeViewIndex < 0 || _activeViewIndex >= Views.Count)
			{
				throw new InvalidOperationException(
					"The ActiveViewIndex is not set to a valid View control.");
			}
			return Views[_activeViewIndex];
		}

		public void SetActiveView(View view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));

			var index = Views.IndexOf(view);
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(view),
					"The specified View is not part of this MultiView.");
			}
			ActiveViewIndex = index;
		}

		internal void RegisterView(View view)
		{
			if (!Views.Contains(view))
			{
				Views.Add(view);

				var index = Views.IndexOf(view);
				if (index == _activeViewIndex)
				{
					view.Visible = true;
					view.NotifyActivated();
				}
				else
				{
					view.Visible = false;
				}
			}
		}

		private void UpdateActiveView(int oldIndex, int newIndex)
		{
			if (oldIndex >= 0 && oldIndex < Views.Count)
			{
				Views[oldIndex].Visible = false;
				Views[oldIndex].NotifyDeactivated();
			}

			if (newIndex >= 0 && newIndex < Views.Count)
			{
				Views[newIndex].Visible = true;
				Views[newIndex].NotifyActivated();
			}

			OnActiveViewChanged.InvokeAsync(EventArgs.Empty);
		}

		protected override void OnBubbledEvent(object sender, EventArgs args)
		{
			base.OnBubbledEvent(sender, args);
		}
	}
}
