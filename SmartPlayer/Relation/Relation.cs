// -----------------------------------------------------------------------
// <copyright file="Relation.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Relation {

	abstract class Relation {
		protected static int? _titleId;
		protected static int? _userId;

		protected static async Task SetData() {
			_titleId = await Service.GetTitleId(MusicPlayer.ActualSong.Title);
			_userId = await Service.GetUserId(MusicPlayer.ActualUser.GetLogin());
		}
	}

}
