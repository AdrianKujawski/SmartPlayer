using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPlayer.PlayerService;

namespace SmartPlayer.Controller {
	class DBConnect {

		public async void Test()
		{
			MPServiceSoapClient soapClient = new MPServiceSoapClient();
			var response = await soapClient.GetUserAsync();
			var lol = response.Body.GetUserResult;
		}
	}
}
