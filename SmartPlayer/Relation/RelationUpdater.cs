// -----------------------------------------------------------------------
// <copyright file="RelationUpdater.cs" company="Adrian Kujawski">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Relation {

	class RelationUpdater : Relation {
		static int? _userSongId;
		static int? _songQty;

		public static async Task UpdateSongQty() {
			await SetSongQty();
			if (_userSongId == null)
				await IsRelationExist();
			await Service.UpdateUserSongQty(_userSongId, _songQty + 1);
		}

		public static async Task<bool> IsRelationExist() {
			await SetData();
			_userSongId = await Service.FindUserSongId(_userId, _titleId);
			if (_userSongId == null)
				return false;

			return true;
		}

		static async Task SetSongQty() {
			_songQty = await Service.GetUserSongQty(_userId, _titleId) ?? 0;
		}
	}

}
