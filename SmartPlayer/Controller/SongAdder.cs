using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using SmartPlayer.Model;

namespace SmartPlayer.Controller
{
	internal class SongAdder
	{
		private readonly Song _song;

		public SongAdder(Song song)
		{
			_song = song;
		}

		private event deleg _thingsToDo;

		public async void AddSong()
		{
			var result = await Service.GetArtistId(_song.Artist);

			if (result == null)
				_thingsToDo += InsertArtist;

			result = await Service.GetAlbumId(_song.Album);
			if (result == null)
				_thingsToDo += InsertAlbum;

			result = await Service.GetTitleId(_song.Title);
			if (result == null)
				_thingsToDo += InsertTitle;

			_thingsToDo.Invoke();
		}


		private async void InsertAlbum()
		{
			int? artistId;
			do
			{
				artistId = await Service.GetArtistId(_song.Artist);
			} while (artistId == null);
			await Service.InsertAlbum(_song.Album, artistId);
		}

		private async void InsertTitle()
		{
			int? albumId;
			do
			{
				albumId = await Service.GetAlbumId(_song.Album);
			} while (albumId == null);

			await Service.InsertTitle(_song.Title, albumId);
		}

		private async void InsertArtist()
		{
			await Service.InsertArtist(_song.Artist);
		}

		private delegate void deleg();
	}
}