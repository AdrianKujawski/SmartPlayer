using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPlayer.Model;
using SmartPlayer.PlayerService;

namespace SmartPlayer.Controller {
	class Service
	{
		private static MpServiceSoapClient _soapClient;


		public static async Task<bool> IsSongExist(string title, string album, string artist)
		{
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.FindSongAsync(title, album, artist);
			return result.Body.FindSongResult;
		}

		public static async Task<int?> GetArtistId(string artist)
		{
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.FindArtistIdAsync(artist);
			return result.Body.FindArtistIdResult;
		}

		public static async Task<int?> GetAlbumId(string album) {
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.FindAlbumIdAsync(album);
			return result.Body.FindAlbumIdResult;
		}

		public static async Task<int?> GetTitleId(string title) {
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.FindTitleIdAsync(title);
			return result.Body.FindTitleIdResult;
		}

		public static async Task<bool> InsertArtist(string artist)
		{
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.InsertArtistAsync(artist);
			return result.Body.InsertArtistResult;
		}

		public static async Task<bool> InsertAlbum(string album, int? artistId) {
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.InsertAlbumAsync(album, artistId);
			return result.Body.InsertAlbumResult;
		}

		public static async Task<bool> InsertTitle(string title, int? albumId) {
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.InsertTitleAsync(title, albumId);
			return result.Body.InsertTitleResult;
		}

		public static async Task<int?> GetUserId(string login) {
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.FindUserIdAsync(login);
			return result.Body.FindUserIdResult;
		}

		public static async Task<bool> InsertNewUserSong(int? userId, int? titleId) {
			_soapClient = new MpServiceSoapClient();
			return await _soapClient.InsertNewUserSongAsync(userId, titleId);
		}

		public static async Task<int?> FindUserSongId(int? userId, int? titleId) {
			_soapClient = new MpServiceSoapClient();
			return await _soapClient.FindUserSongIdAsync(userId, titleId);
		}

		public static async Task<int?> GetUserSongQty(int? userSongId, int? titleId) {
			_soapClient = new MpServiceSoapClient();
			return await _soapClient.GetSongUserQtyAsync(userSongId, titleId);
		}

		public static async Task<bool> UpdateUserSongQty(int? userSongId, int? qty) {
			_soapClient = new MpServiceSoapClient();
			return await _soapClient.UpdateUserSongQtyAsync(userSongId, qty);
		}

		public static async Task<Song[]> GetSongsAndQty(string userLogin) {
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.GetSongsAndQtyAsync(userLogin);
			return result.Body.GetSongsAndQtyResult;
		}

		public static async Task<user> GetUser(string userLogin)
		{
			_soapClient = new MpServiceSoapClient();
			var result = await _soapClient.GetUserAsync(userLogin);
			return result.Body.GetUserResult;
		}
	}
}
