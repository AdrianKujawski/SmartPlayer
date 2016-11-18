// -----------------------------------------------------------------------
// <copyright file="SongFile.cs" company="Unicore">
//     Copyright (c) 2016, Unicore. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartPlayer.Model {

	class SongFile {
		public StorageFile File { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Title { get; set; }
		public BitmapImage AlbumImage { get; set; }

		public string GetFullName() {
			return String.Format("{0} - {1}", Artist, Title);
		}
	}

}
