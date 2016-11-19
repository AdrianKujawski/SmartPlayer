// -----------------------------------------------------------------------
// <copyright file="Service.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using SmartPlayer.PlayerService;

namespace SmartPlayer.Controller {

	static class Service {
		static MpServiceSoapClient SoapClient => new MpServiceSoapClient();

		public static async Task<bool> IsSongExist(string title, string album, string artist) {
			var result = await SoapClient.FindSongAsync(title, album, artist);
			return result.Body.FindSongResult;
		}

		public static async Task<int?> GetArtistId(string artist) {
			var result = await SoapClient.FindArtistIdAsync(artist);
			return result.Body.FindArtistIdResult;
		}

		public static async Task<int?> GetAlbumId(string album) {
			var result = await SoapClient.FindAlbumIdAsync(album);
			return result.Body.FindAlbumIdResult;
		}

		public static async Task<int?> GetTitleId(string title) {
			var result = await SoapClient.FindTitleIdAsync(title);
			return result.Body.FindTitleIdResult;
		}

		public static async Task<bool> InsertArtist(string artist) {
			var result = await SoapClient.InsertArtistAsync(artist);
			return result.Body.InsertArtistResult;
		}

		public static async Task<bool> InsertAlbum(string album, int? artistId) {
			var result = await SoapClient.InsertAlbumAsync(album, artistId);
			return result.Body.InsertAlbumResult;
		}

		public static async Task<bool> InsertTitle(string title, int? albumId) {
			var result = await SoapClient.InsertTitleAsync(title, albumId);
			return result.Body.InsertTitleResult;
		}

		public static async Task<int?> GetUserId(string login) {
			var result = await SoapClient.FindUserIdAsync(login);
			return result.Body.FindUserIdResult;
		}

		public static async Task<bool> InsertNewUserSong(int? userId, int? titleId) {
			return await SoapClient.InsertNewUserSongAsync(userId, titleId);
		}

		public static async Task<int?> FindUserSongId(int? userId, int? titleId) {
			return await SoapClient.FindUserSongIdAsync(userId, titleId);
		}

		public static async Task<int?> GetUserSongQty(int? userSongId, int? titleId) {
			return await SoapClient.GetSongUserQtyAsync(userSongId, titleId);
		}

		public static async Task<bool> UpdateUserSongQty(int? userSongId, int? qty) {
			return await SoapClient.UpdateUserSongQtyAsync(userSongId, qty);
		}

		public static async Task<PlayerService.Song[]> GetSongsAndQty(string userLogin) {
			var result = await SoapClient.GetSongsAndQtyAsync(userLogin);
			return result.Body.GetSongsAndQtyResult;
		}

		public static async Task<user> GetUser(string userLogin) {
			var result = await SoapClient.GetUserAsync(userLogin);
			return result.Body.GetUserResult;
		}
	}

}
