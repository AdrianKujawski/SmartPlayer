using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Lists {
	class ArtistStatisticList : SongStatisticList {
		static readonly List<PlayerService.Song> __sortedArtist = new List<PlayerService.Song>();

		public new static async Task AddSongs() {
			_statisticCollectionSongs.Clear();
			__sortedArtist.Clear();
			var songs = await GetSongs();

			if (!songs.Any())
				return;

			foreach (var song in songs) {
				_statisticCollectionSongs.Add(song);
			}

			GroupByArtist();

			_statisticCollectionSongs = new ObservableCollection<PlayerService.Song>(__sortedArtist);
			Sort();
		}

		static void GroupByArtist() {
				var query = _statisticCollectionSongs.GroupBy(x => x.Artist, x => x.Qty);
				foreach (var album in query) {
					var artistKey = album.Key;
					var artistQty = album.Sum();
					var line = new PlayerService.Song { Title = artistKey, Qty = artistQty };
					__sortedArtist.Add(line);
				}
			}
		}
	}
