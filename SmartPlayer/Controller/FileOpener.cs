// -----------------------------------------------------------------------
// <copyright file="FileOpener.cs">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace SmartPlayer.Controller {

	class FileOpener {
		readonly FileOpenPicker _openPicker;

		public FileOpener() {
			_openPicker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.MusicLibrary };
			_openPicker.FileTypeFilter.Add(".wmv");
			_openPicker.FileTypeFilter.Add(".wma");
			_openPicker.FileTypeFilter.Add(".mp3");
		}

		public async Task<IReadOnlyCollection<StorageFile>> Open() {
			return await _openPicker.PickMultipleFilesAsync();
		}
	}

}
