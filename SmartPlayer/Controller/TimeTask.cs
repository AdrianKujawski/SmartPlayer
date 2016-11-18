using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SmartPlayer.Controller {
	class TimeTask {
		DispatcherTimer _dispatcher;

		public TimeTask() {
			_dispatcher = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
		}

		public void AddMethod(EventHandler<object> method) {
			_dispatcher.Tick += method;
		}

		public void Start() {
			if(_dispatcher != null)
				_dispatcher.Start();
		}

		public void Stop() {
			if (_dispatcher != null)
				_dispatcher.Stop();
		}
	}
}
