// -----------------------------------------------------------------------
// <copyright file="User.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Model {

	sealed class User {
		readonly string _login;
		readonly string _password;
		string _name;

		public User(string login, string password) {
			_login = login;
			_password = password;
		}

		public async Task<bool> CheckLoginAndPassword() {
			var user = await Service.GetUser(_login);
			if (user == null)
				return false;

			if (user.login != _login || user.password != _password) return false;

			_name = user.name;
			return true;
		}

		public string GetLogin() {
			return _login;
		}

		public string GetName() {
			return _name;
		}
	}

}
