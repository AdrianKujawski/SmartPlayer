// -----------------------------------------------------------------------
// <copyright file="StatisticList.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Lists {

	class SongStatisticList {
		protected static ObservableCollection<PlayerService.Song> _statisticCollectionSongs = new ObservableCollection<PlayerService.Song>();

		public static async Task AddSongs() {
			_statisticCollectionSongs.Clear();
			var songs = await GetSongs();

			if (!songs.Any())
				return;

			foreach (var song in songs) {
				_statisticCollectionSongs.Add(song);
			}
			Sort();
		}

		protected static async Task<PlayerService.Song[]> GetSongs() {
			var login = MusicPlayer.ActualUser.GetLogin();
			var songs = await Service.GetSongsAndQty(login);
			return songs;
		}

		public static ObservableCollection<PlayerService.Song> GetList() {
			return _statisticCollectionSongs;
		}

		protected static void Sort() {
			var sortedList = _statisticCollectionSongs.OrderByDescending(x => x.Qty);
			_statisticCollectionSongs = new ObservableCollection<PlayerService.Song>(sortedList);
		}
	}

}
