using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using SmartPlayer.Controller;
using SmartPlayer.Model;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartPlayer
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private DispatcherTimer _dispatcherTimer;
		private DispatcherTimer _timerSlider;
		private DateTimeOffset _startTime;
		private DateTimeOffset _lastTime;

		public MainPage()
		{
			InitializeComponent();
			volumeSlider.Value = 100;
			ListViewCreate();
			listViewSongs.Tapped += mediaPlayer_ItemClick;
		}

		private void ListViewCreate()
		{
			MusicPlayer.songs = new ObservableCollection<Song>();
			listViewSongs.ItemsSource = MusicPlayer.songs;
		}
		private async void button_Click(object sender, RoutedEventArgs e)
		{
			await SetLocalMedia();
		}

		async private Task SetLocalMedia()
		{
			var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

			openPicker.FileTypeFilter.Add(".wmv");
			openPicker.FileTypeFilter.Add(".wma");
			openPicker.FileTypeFilter.Add(".mp3");

			var file = await openPicker.PickMultipleFilesAsync();

			if (file != null)
			{
				MusicPlayer.AddSongToPlaylist(file);
				listViewSongs.ItemsSource = MusicPlayer.songs;
			}

			file = null;
		}

		private void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
		{
			int temp = (int) mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;

			timelineSlider.Value = 0;
			timelineSlider.Maximum = temp;

			mediaPlayer.Volume = volumeSlider.Value;
			mediaPlayer.Play();
			StartCountTimeSong();
		}

		private void StartCountTimeSong()
		{
			if (_dispatcherTimer == null)
			{
				_dispatcherTimer = new DispatcherTimer();
				_dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
				_dispatcherTimer.Tick += SetCurrentSongTime;
				_dispatcherTimer.Tick += ChangeSlidePosition;

				_startTime = DateTimeOffset.Now;
				_lastTime = _startTime;
				_dispatcherTimer.Start();
			}
		}

		private void ChangeSlidePosition(object sender, object e)
		{
			timelineSlider.Value += 1;
		}

		private void SetCurrentSongTime(object sender, object e)
		{
			ChangeTimeTextValue();
		}

		private void SeekToMediaPosition(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
		{
			int sliderValue = (int) timelineSlider.Value;

			TimeSpan ts = new TimeSpan(0, 0, 0, sliderValue, 0);
			mediaPlayer.Position = ts;
			ChangeTimeTextValue();
		}

		private void ChangeTimeTextValue()
		{
			var songTime = mediaPlayer.Position.ToString(@"hh\:mm\:ss");
			time.Text = songTime;
		}

		private void ChangeMediaVolume(object sender, RangeBaseValueChangedEventArgs e)
		{
			mediaPlayer.Volume = volumeSlider.Value;
		}

		private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
		{
			time.Text = "00:00:00";
			timelineSlider.Value = 0;
			mediaPlayer.Play();
		}

		private async void mediaPlayer_ItemClick(object sender, RoutedEventArgs e)
		{
			try {

			var selectedSong = listViewSongs.SelectedItem as Song;
			mediaPlayer.SetSource(await selectedSong.File.OpenAsync(FileAccessMode.Read), selectedSong.File.ContentType);
			}
			catch(Exception exc)
			{
				MessageDialog saa = new MessageDialog(exc.ToString());
				saa.ShowAsync();
			}
		}
	}
}