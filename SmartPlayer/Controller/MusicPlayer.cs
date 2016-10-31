using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Windows.Storage;
using SmartPlayer.Model;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartPlayer.Controller
{
	internal class MusicPlayer
	{
		public static ObservableCollection<Song> Songs { set; get; }
		private static Song _song;
		public static void AddSongToPlaylist(IEnumerable<StorageFile> files)
		{
			CreateSong(files);
		}

		private static async void CreateSong(IEnumerable<StorageFile> files)
		{
			foreach (var file in files)
			{
				var newSong = new Song();
				var properties = await file.Properties.GetMusicPropertiesAsync();
				var artist = properties.Artist;
				var title = properties.Title;
				var album = properties.Album;
				var img = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.ListView);

				var isArtist = string.IsNullOrWhiteSpace(artist);
				var isTitle = string.IsNullOrWhiteSpace(title);
				var isAlbum = string.IsNullOrWhiteSpace(album);

				newSong.File = file;

				BitmapImage lol = new BitmapImage();
				lol.SetSource(img);
				newSong.AlbumImage = lol;

				if (!isAlbum)
					newSong.Album = album;

				if (!isArtist)
					newSong.Artist = artist;
						
				if (!isTitle)
					newSong.Title = title;
				else 
					newSong.Title = file.DisplayName;

				Songs.Add(newSong);
			}
		}
	}
}