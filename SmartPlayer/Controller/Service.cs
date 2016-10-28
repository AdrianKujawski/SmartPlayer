using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}
