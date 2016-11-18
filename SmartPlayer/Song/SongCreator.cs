// -----------------------------------------------------------------------
// <copyright file="SongCreator.cs" company="Adrian Kujawski">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using SmartPlayer.Model;

namespace SmartPlayer.Song {

	class SongCreator {
		static SongFile _newSong;
		static MusicProperties _properties;
		static string _artist;
		static string _title;
		static string _album;
		static bool _isArtist;
		static bool _isTitle;
		static bool _isAlbum;

		public static async Task<SongFile> Create(StorageFile file) {
			await GetProperties(file);
			CheckNullsOrWhiteSpace();
			await CreateSong(file);
			return _newSong;
		}

		static async Task GetProperties(StorageFile file) {
			_properties = await file.Properties.GetMusicPropertiesAsync();
			_artist = _properties.Artist;
			_title = _properties.Title;
			_album = _properties.Album;
		}

		static void CheckNullsOrWhiteSpace() {
			_isArtist = string.IsNullOrWhiteSpace(_artist);
			_isTitle = string.IsNullOrWhiteSpace(_title);
			_isAlbum = string.IsNullOrWhiteSpace(_album);
		}

		static async Task CreateSong(StorageFile file) {
			_newSong = new SongFile();
			_newSong.File = file;
			_newSong.AlbumImage = await GetBitmapImage(file);

			if (!_isAlbum)
				_newSong.Album = _album;

			if (!_isArtist)
				_newSong.Artist = _artist;

			_newSong.Title = !_isTitle ? _title : file.DisplayName;
		}

		static async Task<BitmapImage> GetBitmapImage(StorageFile file) {
			var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.ListView);
			var bitmapImage = new BitmapImage();
			bitmapImage.SetSource(thumbnail);
			return bitmapImage;
		}
	}

}
