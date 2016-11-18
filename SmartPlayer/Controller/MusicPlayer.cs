// -----------------------------------------------------------------------
// <copyright file="MusicPlayer.cs" company="Adrian Kujawski">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SmartPlayer.Model;

namespace SmartPlayer.Controller {

	static class MusicPlayer {
		public static User ActualUser { get; set; }
		public static SongFile ActualSong { get; set; }
		public static bool IsMute { get; set; }
		public static double TempVolumeSlider { get; set; }
		public static double TempVolumeValue { get; set; }
		
	}

}
