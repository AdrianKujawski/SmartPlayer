using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using SmartPlayer.Controller;
using SmartPlayer.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartPlayer
{
	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public enum NotifyType
		{
			StatusMessage,
			ErrorMessage
		}

		private DispatcherTimer _dispatcherTimer;
		private double _duractionSong;
		private DateTimeOffset _lastTime;
		private DateTimeOffset _startTime;
		private DispatcherTimer _timerSlider;
		private int actualSecond;
		private double halfDuractionSong;

		public MainPage()
		{
			InitializeComponent();
			VolumeSlider.Value = 100;
			ListViewCreate();
			ListViewSongs.Tapped += mediaPlayer_ItemClick;
			SongName.Text = "";
		}

		public void test(object sender, object e)
		{
			if (actualSecond >= halfDuractionSong)
				NotifyUser("Piosenka przesluchana!", NotifyType.StatusMessage);
			Debug.WriteLine(halfDuractionSong);
			Debug.WriteLine(++actualSecond);
		}

		private void ListViewCreate()
		{
			MusicPlayer.Songs = new ObservableCollection<Song>();
			ListViewSongs.ItemsSource = MusicPlayer.Songs;
		}

		private async void button_Click(object sender, RoutedEventArgs e)
		{
			await SetLocalMedia();
		}

		private async Task SetLocalMedia()
		{
			var openPicker = new FileOpenPicker();

			openPicker.FileTypeFilter.Add(".wmv");
			openPicker.FileTypeFilter.Add(".wma");
			openPicker.FileTypeFilter.Add(".mp3");

			var file = await openPicker.PickMultipleFilesAsync();

			if (file != null)
			{
				MusicPlayer.AddSongToPlaylist(file);
				ListViewSongs.ItemsSource = MusicPlayer.Songs;
			}
		}

		private void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
		{
			SetDurationSong();
			TimelineSlider.Value = 0;
			TimelineSlider.Maximum = _duractionSong;

			MediaPlayer.Volume = VolumeSlider.Value;
			MediaPlayer.Play();
			StartCountTimeSong();
		}

		private void SetDurationSong()
		{
			actualSecond = 0;
			_duractionSong = (int) MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
			halfDuractionSong = Math.Floor(_duractionSong/2);
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
				_dispatcherTimer.Tick += test;
				_dispatcherTimer.Start();
			}
		}

		private void ChangeSlidePosition(object sender, object e)
		{
			TimelineSlider.Value += 1;
		}

		private void SetCurrentSongTime(object sender, object e)
		{
			ChangeTimeTextValue();
		}

		private void SeekToMediaPosition(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
		{
			var sliderValue = (int) TimelineSlider.Value;

			var ts = new TimeSpan(0, 0, 0, sliderValue, 0);
			MediaPlayer.Position = ts;
			ChangeTimeTextValue();
		}

		private void ChangeTimeTextValue()
		{
			var songTime = MediaPlayer.Position.ToString(@"hh\:mm\:ss");
			Time.Text = songTime;
		}

		private void ChangeMediaVolume(object sender, RangeBaseValueChangedEventArgs e)
		{
			MediaPlayer.Volume = VolumeSlider.Value;
		}

		private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
		{
			Time.Text = "00:00:00";
			TimelineSlider.Value = 0;
			SetDurationSong();
			MediaPlayer.Play();
		}

		private async void mediaPlayer_ItemClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var selectedSong = ListViewSongs.SelectedItem as Song;
				MediaPlayer.SetSource(await selectedSong.File.OpenAsync(FileAccessMode.Read), selectedSong.File.ContentType);
				SongName.Text = selectedSong.GetFullName();

				var isSongExist = await Service.IsSongExist(selectedSong.Title, selectedSong.Album, selectedSong.Artist);

				if (isSongExist)
					NotifyUser(selectedSong.Title + " - istnieje.", NotifyType.StatusMessage);
				else
				{
					NotifyUser("Piosenka zostanie dodana do bazy...", NotifyType.ErrorMessage);
					var adder = new SongAdder(selectedSong);
					try
					{
						adder.AddSong();
						NotifyUser(selectedSong.Title + " - dodano do bazy.", NotifyType.StatusMessage);
					}
					catch (Exception exc)
					{
						NotifyUser("Coś poszło nie tak...", NotifyType.ErrorMessage);
					}
				}
			}
			catch (Exception exc)
			{
			}
		}

		public void NotifyUser(string strMessage, NotifyType type)
		{
			switch (type)
			{
				case NotifyType.StatusMessage:
					StatusBorder.Background = new SolidColorBrush(Colors.Green);
					break;
				case NotifyType.ErrorMessage:
					StatusBorder.Background = new SolidColorBrush(Colors.Red);
					break;
			}
			StatusText.Text = strMessage;
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Debug.WriteLine(MainLayout.ActualHeight);
		}

		private void StatisticButton_Click(object sender, RoutedEventArgs e)
		{
			var isPlayerActiv = PlayerPanel.Visibility == Visibility.Visible;

			if (isPlayerActiv)
			{
				PlayerPanel.Visibility = Visibility.Collapsed;
				StatisticPanel.Visibility = Visibility.Visible;
			}
			else
			{
				PlayerPanel.Visibility = Visibility.Visible;
				StatisticPanel.Visibility = Visibility.Collapsed;
			}
		}
	}
}