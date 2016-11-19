// -----------------------------------------------------------------------
// <copyright file="PlayerController.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Windows.UI.Xaml.Controls;

namespace SmartPlayer.Controller {

	static class PlayerController {
		public static MediaElement MediaPlayer { private get; set; }

		public static void PlaySong() {
			MediaPlayer.Play();
			TimeTask.Start();
		}

		public static void PauseSong() {
			MediaPlayer.Pause();
			TimeTask.Stop();
		}

		public static void StopSong() {
			MediaPlayer.Stop();
			TimeTask.Stop();
		}
	}

}
