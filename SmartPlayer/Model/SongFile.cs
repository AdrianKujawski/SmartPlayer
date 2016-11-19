// -----------------------------------------------------------------------
// <copyright file="SongFile.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartPlayer.Model {

	sealed class SongFile {
		public StorageFile File { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Title { get; set; }
		public BitmapImage AlbumImage { get; set; }

		public string GetFullName() {
			return string.Format("{0} - {1}", Artist, Title);
		}
	}

}
