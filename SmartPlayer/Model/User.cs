using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Model {
	class User
	{
		private string _login;
		private string _password;
		private string _name;
		public User(string login, string password)
		{
			_login = login;
			_password = password;
		}

		public async Task<bool> CheckLoginAndPassword()
		{
			var user = await Service.GetUser(_login);
			if (user == null)
				return false;

			if (user.Login == _login && user.Password == _password)
			{
				_name = user.Name;
				return true;
			}
			return false;
			
		}

		public string GetLogin()
		{
			return _login;
		}

		public string GetName()
		{
			return _name;
		}
	}
}
