using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPlayer.PlayerService;

namespace SmartPlayer.Controller {
	class Service
	{
		private static MPServiceSoapClient _soapClient;


		public static async Task<bool> IsSongExist(string title, string album, string artist)
		{
			_soapClient = new MPServiceSoapClient();
			var response = await _soapClient.FindSongAsync(title, album, artist);
			return response.Body.FindSongResult;
		}
	}
}
