// -----------------------------------------------------------------------
// <copyright file="Notify.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SmartPlayer.NotifyFolder {

	static class Notify {
		public static Border Border { private get; set; }
		public static TextBlock TextBlock { private get; set; }

		public static void SetNotify(string notifyText, NotifyType type) {
			SetBackground(type);
			SetText(notifyText);
		}

		static void SetBackground(NotifyType type) {
			switch (type) {
				case NotifyType.StatusMessage:
					Border.Background = new SolidColorBrush(Colors.Green);
					break;
				case NotifyType.ErrorMessage:
					Border.Background = new SolidColorBrush(Colors.Red);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		static void SetText(string notifyText) {
			TextBlock.Text = notifyText;
		}
	}

}
