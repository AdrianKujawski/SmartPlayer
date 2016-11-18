// -----------------------------------------------------------------------
// <copyright file="MusicPlayer.cs" company="Unicore">
//     Copyright (c) 2016, Unicore. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using SmartPlayer.Model;
using SmartPlayer.PlayerService;

namespace SmartPlayer.Controller {

	static class MusicPlayer {
		public static ObservableCollection<SongFile> Songs { set; get; }
		public static ObservableCollection<Song> StatisticCollectionSongs { get; set; }

		public static User ActualUser { get; set; }
		public static SongFile ActualSong { get; set; }
		public static bool IsMute { get; set; }

		public static double TempVolumeSlider { get; set; }
		public static double TempVolumeValue { get; set; }

		public static void AddSongToPlaylist(IEnumerable<StorageFile> files) {
			CreateSong(files);
		}

		static async void CreateSong(IEnumerable<StorageFile> files) {
			foreach (var file in files) {
				var newSong = new SongFile();
				var properties = await file.Properties.GetMusicPropertiesAsync();
				var artist = properties.Artist;
				var title = properties.Title;
				var album = properties.Album;
				var img = await file.GetThumbnailAsync(ThumbnailMode.ListView);

				var isArtist = string.IsNullOrWhiteSpace(artist);
				var isTitle = string.IsNullOrWhiteSpace(title);
				var isAlbum = string.IsNullOrWhiteSpace(album);

				newSong.File = file;

				var lol = new BitmapImage();
				lol.SetSource(img);
				newSong.AlbumImage = lol;

				if (!isAlbum)
					newSong.Album = album;

				if (!isArtist)
					newSong.Artist = artist;

				newSong.Title = !isTitle ? title : file.DisplayName;

				Songs.Add(newSong);
			}
		}

		public static async Task AddSongsToStatisticView() {
			if (StatisticCollectionSongs == null)
				StatisticCollectionSongs = new ObservableCollection<Song>();

			StatisticCollectionSongs.Clear();
			var songs = await Service.GetSongsAndQty(ActualUser.GetLogin());

			if (!songs.Any())
				return;

			foreach (var song in songs) {
				StatisticCollectionSongs.Add(song);
			}
		}

		public static void Sort<T>(ObservableCollection<T> collection) {
			var sorted = collection.OrderBy(x => x).ToList();
			for (var i = 0; i < sorted.Count(); i++)
				collection.Move(collection.IndexOf(sorted[i]), i);
		}
	}

}
