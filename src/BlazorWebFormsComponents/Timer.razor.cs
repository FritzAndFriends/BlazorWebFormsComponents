using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public partial class Timer : BaseWebFormsComponent, IDisposable
	{
		private System.Threading.Timer _timer;
		private readonly object _lock = new();
		private bool _disposed;

		/// <summary>
		/// The interval, in milliseconds, between ticks. Default is 60000 (60 seconds).
		/// </summary>
		[Parameter]
		public int Interval { get; set; } = 60000;

		// Enabled is inherited from BaseWebFormsComponent (default: true).
		// Timer uses it to control whether ticking occurs.

		/// <summary>
		/// Occurs when the number of milliseconds specified in the Interval property has elapsed.
		/// </summary>
		[Parameter]
		public EventCallback OnTick { get; set; }

		protected override void OnAfterRender(bool firstRender)
		{
			base.OnAfterRender(firstRender);

			if (firstRender)
			{
				ConfigureTimer();
			}
		}

		protected override void OnParametersSet()
		{
			base.OnParametersSet();
			ConfigureTimer();
		}

		private void ConfigureTimer()
		{
			lock (_lock)
			{
				if (_disposed) return;

				if (Enabled && Interval > 0)
				{
					if (_timer == null)
					{
						_timer = new System.Threading.Timer(OnTimerCallback, null, Interval, Interval);
					}
					else
					{
						_timer.Change(Interval, Interval);
					}
				}
				else
				{
					_timer?.Change(Timeout.Infinite, Timeout.Infinite);
				}
			}
		}

		private async void OnTimerCallback(object state)
		{
			lock (_lock)
			{
				if (_disposed) return;
			}

			await InvokeAsync(async () =>
			{
				if (OnTick.HasDelegate)
				{
					await OnTick.InvokeAsync();
				}
				StateHasChanged();
			});
		}

		void IDisposable.Dispose()
		{
			lock (_lock)
			{
				_disposed = true;
				_timer?.Dispose();
				_timer = null;
			}
		}
	}
}
