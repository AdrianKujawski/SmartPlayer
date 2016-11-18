using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Lists {
	class StatisticList {
		static readonly ObservableCollection<PlayerService.Song> __statisticCollectionSongs = new ObservableCollection<PlayerService.Song>();

		public static async Task AddSongsToStatisticView() {
			__statisticCollectionSongs.Clear();
			var songs = await GetSongs();

			if (!songs.Any())
				return;

			foreach (var song in songs) {
				__statisticCollectionSongs.Add(song);
			}
		}

		static async Task<PlayerService.Song[]> GetSongs() {
			var login = MusicPlayer.ActualUser.GetLogin();
			var songs = await Service.GetSongsAndQty(login);
			return songs;
		}

		public static ObservableCollection<PlayerService.Song> GetList() {
			return __statisticCollectionSongs;
		}

		static void Sort<T>(ObservableCollection<T> collection) {
			var sorted = collection.OrderBy(x => x).ToList();
			for (var i = 0; i < sorted.Count(); i++)
				collection.Move(collection.IndexOf(sorted[i]), i);
		}
	}
}
