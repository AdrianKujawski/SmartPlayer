using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.ServiceDiscovery.Dnssd;

namespace SmartPlayer.Controller {
	class RelationUpdater
	{
		private static int? _titleId;
		private static int? _userId;
		private static int? _userSongId;
		private static int? _songQty;
		private static bool test = false;

		public static async Task AddUserSongRelation()
		{
			await SetData();
			await Service.InsertNewUserSong(_userId, _titleId);
		}

		public static async Task<bool> IsRelationExist()
		{
			await SetData();
			_userSongId = await Service.FindUserSongId(_userId, _titleId);
			if (_userSongId == null)
				return false;
			return true;
		}

		private static async Task SetSongQty()
		{
			_songQty = await Service.GetUserSongQty(_userId, _titleId) ?? 0;
		}

		public static async Task UpdateSongQty()
		{
			await SetSongQty();
			if (_userSongId == null)
				await IsRelationExist();
			await Service.UpdateUserSongQty(_userSongId, _songQty + 1);
		}

		private static async Task<bool> SetData()
		{
				_titleId = await Service.GetTitleId(MusicPlayer.ActualSong.Title);
				_userId = await Service.GetUserId(MusicPlayer.ActualUser.GetLogin());;

			return true;
		}
	}
}
