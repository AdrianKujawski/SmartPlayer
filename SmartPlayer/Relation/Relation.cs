// -----------------------------------------------------------------------
// <copyright file="Relation.cs" company="Adrian Kujawski">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Relation {

	class Relation {
		protected static int? _titleId;
		protected static int? _userId;

		protected static async Task<bool> SetData() {
			_titleId = await Service.GetTitleId(MusicPlayer.ActualSong.Title);
			_userId = await Service.GetUserId(MusicPlayer.ActualUser.GetLogin());
			;

			return true;
		}
	}

}
