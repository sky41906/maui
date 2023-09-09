using System.Threading.Tasks;
using Microsoft.Maui.Animations;

namespace Microsoft.Maui.Controls.Core.UnitTests
{
	class AsyncTicker : Ticker
	{
		bool _enabled;

		public override bool IsRunning => _enabled; 

		public void SetEnabled(bool enabled)
		{
			SystemEnabled = enabled;
			_enabled = enabled;
		}

		public override async void Start()
		{
			_enabled = true;

			while (_enabled)
			{
				Fire?.Invoke();
				await Task.Delay(16);
			}
		}

		public override void Stop()
		{
			_enabled = false;
		}
	}
}