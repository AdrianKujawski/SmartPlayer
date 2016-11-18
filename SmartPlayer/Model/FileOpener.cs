using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace SmartPlayer.Model {
	class FileOpener {
		FileOpenPicker openPicker;

		public FileOpener() {
			openPicker = new FileOpenPicker();
			openPicker.FileTypeFilter.Add(".wmv");
			openPicker.FileTypeFilter.Add(".wma");
			openPicker.FileTypeFilter.Add(".mp3");
		}

		public async Task<IReadOnlyCollection<StorageFile>> Open() {
			return await openPicker.PickMultipleFilesAsync();
		}
	}
}
