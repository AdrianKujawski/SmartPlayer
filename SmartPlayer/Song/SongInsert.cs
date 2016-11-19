// -----------------------------------------------------------------------
// <copyright file="SongInsert.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using SmartPlayer.Controller;
using SmartPlayer.Model;
using SmartPlayer.NotifyFolder;

namespace SmartPlayer.Song {

	static class SongInsert {
		static SongFile _song;

		public static async Task<bool> AddSong(SongFile song) {
			_song = song;
			try {
				var result = await Service.GetArtistId(_song.Artist);

				if (result == null)
					await InsertArtist();

				result = await Service.GetAlbumId(_song.Album);
				if (result == null)
					await InsertAlbum();

				result = await Service.GetTitleId(_song.Title);
				if (result == null)
					await InsertTitle();
			}
			catch (Exception) {
				Notify.SetNotify("Błąd podczas dodawania utworu do bazdy.", NotifyType.ErrorMessage);
				return false;
			}

			return true;
		}

		static async Task InsertArtist() {
			await Service.InsertArtist(_song.Artist);
		}

		static async Task InsertAlbum() {
			var artistId = await Service.GetArtistId(_song.Artist);
			await Service.InsertAlbum(_song.Album, artistId);
		}

		static async Task InsertTitle() {
			var albumId = await Service.GetAlbumId(_song.Album);
			await Service.InsertTitle(_song.Title, albumId);
		}
	}

}
