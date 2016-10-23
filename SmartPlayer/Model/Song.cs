using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartPlayer.Model {
	public class Song {
		public StorageFile File { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Title { get; set; }
		public BitmapImage AlbumImage { get; set; }

		public string GetFullName()
		{
			return String.Format("{0} - {1}", Artist, Title);
		}
	}
}