// -----------------------------------------------------------------------
// <copyright file="TimeTask.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Windows.UI.Xaml;

namespace SmartPlayer.Controller {

	static class TimeTask {
		public static DispatcherTimer DispatcherTimer { get; set; }

		public static void AddMethod(EventHandler<object> method) {
			DispatcherTimer.Tick += method;
		}

		public static void Start() {
			DispatcherTimer?.Start();
		}

		public static void Stop() {
			DispatcherTimer?.Stop();
		}

		public static void Disable() {
			DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
		}
	}

}
