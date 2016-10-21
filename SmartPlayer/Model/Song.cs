using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Controls;

namespace SmartPlayer.Model
{
	public class Song
	{
		public StorageFile File { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Title { get; set; }
		public Image AlbumImage { get; set; }
	}
}