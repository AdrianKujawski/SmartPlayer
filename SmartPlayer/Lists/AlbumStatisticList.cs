// -----------------------------------------------------------------------
// <copyright file="SongStatisticList - Copy.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Lists {

	class AlbumStatisticList : SongStatisticList {
		static readonly List<PlayerService.Song> __sortedAlbums = new List<PlayerService.Song>();

		public new static async Task AddSongs() {
			_statisticCollectionSongs.Clear();
			__sortedAlbums.Clear();
			var songs = await GetSongs();

			if (!songs.Any())
				return;

			foreach (var song in songs) {
				_statisticCollectionSongs.Add(song);
				
			}

			GroupByAlbum();
			
			_statisticCollectionSongs = new ObservableCollection<PlayerService.Song>(__sortedAlbums);
			Sort();
		}

		static void GroupByAlbum() {
			var query = _statisticCollectionSongs.GroupBy(x => x.Album, x => x.Qty);
			foreach (var album in query) {
				var albumKey = album.Key;
				var albumsQty = album.Sum();
				var artist = _statisticCollectionSongs.FirstOrDefault(x => x.Album == albumKey).Artist;
				var line = new PlayerService.Song { Artist = artist, Title = albumKey, Qty = albumsQty };
				__sortedAlbums.Add(line);
			}
		}
	}

}
