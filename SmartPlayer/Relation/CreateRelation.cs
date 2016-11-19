// -----------------------------------------------------------------------
// <copyright file="CreateRelation.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using SmartPlayer.Controller;

namespace SmartPlayer.Relation {

	class CreateRelation : Relation {
		public static async Task AddUserSongRelation() {
			await SetData();
			await Service.InsertNewUserSong(_userId, _titleId);
		}
	}

}
