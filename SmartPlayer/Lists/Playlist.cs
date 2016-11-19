// -----------------------------------------------------------------------
// <copyright file="Playlist.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using SmartPlayer.Model;
using SmartPlayer.Song;

namespace SmartPlayer.Lists {

	static class Playlist {
		static readonly ObservableCollection<SongFile> __songs = new ObservableCollection<SongFile>();

		public static async void AddSongToPlaylist(IEnumerable<StorageFile> files) {
			foreach (var file in files) {
				var song = await SongCreator.Create(file);
				__songs.Add(song);
			}
		}

		public static ObservableCollection<SongFile> GetListOfSongs() {
			return __songs;
		}

		public static void Clear() {
			__songs.Clear();
		}
	}

}
