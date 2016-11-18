// -----------------------------------------------------------------------
// <copyright file="SongAdder.cs" company="Unicore">
//     Copyright (c) 2016, Unicore. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using SmartPlayer.Model;

namespace SmartPlayer.Controller {

	class SongAdder {
		readonly SongFile _song;

		public SongAdder(SongFile song) {
			_song = song;
		}

		public async void AddSong() {
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

		async Task InsertArtist() {
			await Service.InsertArtist(_song.Artist);
		}

		async Task InsertAlbum() {
			var artistId = await Service.GetArtistId(_song.Artist);
			await Service.InsertAlbum(_song.Album, artistId);
		}

		async Task InsertTitle() {
			var albumId = await Service.GetAlbumId(_song.Album);
			await Service.InsertTitle(_song.Title, albumId);
		}
	}

}
 