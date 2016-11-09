using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

		private readonly double _songDurationPart = 30;

		private DispatcherTimer _dispatcherTimer;
		private double _duractionSong;
		private DateTimeOffset _lastTime;
		private DateTimeOffset _startTime;
		private DispatcherTimer _timerSlider;
		private int actualSecond;
		private double halfDuractionSong;
		private bool isUpdated;

		public MainPage()
		{
			InitializeComponent();
			VolumeSlider.Value = 100;
			ListViewCreate();
			ListViewSongs.Tapped += mediaPlayer_ItemClick;
			SongName.Text = "";
			isUpdated = false;
			StatusText.Text = "Witaj " + MusicPlayer.ActualUser.GetName();
			MusicPlayer.IsSongPlaying = false;
			MusicPlayer.IsStopped = false;
		}

		public async void Test(object sender, object e)
		{
			Debug.WriteLine("T: {0} >= {1}", actualSecond, halfDuractionSong);
			if ((actualSecond >= halfDuractionSong) && !isUpdated)
			{
				isUpdated = true;
				var isExist = await RelationUpdater.IsRelationExist();
				if (!isExist)
					await RelationUpdater.AddUserSongRelation();
				await RelationUpdater.UpdateSongQty();
				NotifyUser("Piosenka przesluchana!", NotifyType.StatusMessage);
			}
			actualSecond++;
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
			StartCountTimeSong();
		}

		private void SetDurationSong()
		{
			isUpdated = false;
			actualSecond = 0;
			_duractionSong = (int) MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
			halfDuractionSong = Math.Floor(_duractionSong/_songDurationPart);
		}

		private void StartCountTimeSong()
		{
			if (_dispatcherTimer == null)
			{
				_dispatcherTimer = new DispatcherTimer();
				_dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
				_dispatcherTimer.Tick += SetCurrentSongTime;
				_dispatcherTimer.Tick += ChangeSlidePosition;
				_dispatcherTimer.Tick += Test;

				_startTime = DateTimeOffset.Now;
				_lastTime = _startTime;
				_dispatcherTimer.Start();
			}

			if(!_dispatcherTimer.IsEnabled)
				_dispatcherTimer.Start();
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
			StartSongAgain();
		}

		private void StartSongAgain()
		{
			Time.Text = "00:00:00";
			TimelineSlider.Value = 0;
			SetDurationSong();
			MediaPlayer.Play();
		}

		private void mediaPlayer_ItemClick(object sender, RoutedEventArgs e)
		{
			var selectedSong = ListViewSongs.SelectedItem as Song;
			if (selectedSong == null)
			{
				ListViewSongs.SelectedItem = null;
				return;
			}

			SetPlayerSource(selectedSong);
			CheckSongInDataBase(selectedSong);
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

		private async void StatisticButton_Click(object sender, RoutedEventArgs e)
		{
			var isPlayerActiv = PlayerPanel.Visibility == Visibility.Visible;

			if (isPlayerActiv)
			{
				PlayerPanel.Visibility = Visibility.Collapsed;
				StatisticPanel.Visibility = Visibility.Visible;
				await MusicPlayer.ConvertServiceSong();
				StatisticListSong.ItemsSource = MusicPlayer.StatisticCollectionSongs;
			}
			else
			{
				PlayerPanel.Visibility = Visibility.Visible;
				StatisticPanel.Visibility = Visibility.Collapsed;
			}
		}

		private void ChangePlayingState(object sender, RoutedEventArgs e)
		{
			ChangePlayingState();
		}

		private void ChangePlayingState()
		{
			if (MusicPlayer.IsSongPlaying)
				PlaySong();
			else
				PauseSong();

			MusicPlayer.IsSongPlaying = !MusicPlayer.IsSongPlaying;
		}

		private void PauseSong()
		{
			MediaPlayer.Pause();
			PlayPauseButton.Content = "|>";
			_dispatcherTimer.Stop();
		}

		private void PlaySong()
		{
			MediaPlayer.Play();
			PlayPauseButton.Content = "||";
			if (!MusicPlayer.IsStopped)
				_dispatcherTimer.Start();
			else
			{
				StartSongAgain();
				StartCountTimeSong();
				_dispatcherTimer.Start();
				MusicPlayer.IsStopped = false;
			}
		}

		private void NextSong(object sender, RoutedEventArgs e)
		{
			var sign = SetDirectionOfChange(sender);

			if (ListViewSongs.Items.Count != 0)
			{
				var numberOfSong = ListViewSongs.Items.Count - 1;
				if (numberOfSong == 0)
					return;
			
				var currentSongPosition = ListViewSongs.SelectedIndex;
				Song nextSong;
				if (currentSongPosition < numberOfSong)
				{
					currentSongPosition += sign;
					if (currentSongPosition < 0)
						currentSongPosition = numberOfSong;
					nextSong = GetNextSong(currentSongPosition);
				}
				else
				{
					if(sign == 1)
						currentSongPosition = 0;
					else
						currentSongPosition += sign;
					nextSong = GetNextSong(currentSongPosition);
				}

				SetPlayerSource(nextSong);
				MusicPlayer.IsStopped = false;
			}
		}

		private async void SetPlayerSource(Song song)
		{
			MusicPlayer.ActualSong = song;
			MediaPlayer.SetSource(await song.File.OpenAsync(FileAccessMode.Read), song.File.ContentType);
			SongName.Text = song.GetFullName();
			CheckSongInDataBase(song);
		}

		private async void CheckSongInDataBase(Song song)
		{
			var isSongExist = await Service.IsSongExist(song.Title, song.Album, song.Artist);

			if (isSongExist)
				NotifyUser(song.Title + " - istnieje.", NotifyType.StatusMessage);
			else
			{
				NotifyUser("Piosenka zostanie dodana do bazy...", NotifyType.ErrorMessage);
				var adder = new SongAdder(song);
				try
				{
					adder.AddSong();
					NotifyUser(song.Title + " - dodano do bazy.", NotifyType.StatusMessage);
				}
				catch (Exception exc)
				{
					NotifyUser("Coś poszło nie tak...", NotifyType.ErrorMessage);
				}
			}
		}

		private Song GetNextSong(int position)
		{
			ListViewSongs.SelectedIndex = position;
			return ListViewSongs.Items.ElementAt(position) as Song;
		}

		private int SetDirectionOfChange(object sender)
		{
			var buttonName = sender as Button;
			return buttonName != null && buttonName.Name == "NextButton" ? 1 : -1;
		}

		private void StopSong(object sender, RoutedEventArgs e)
		{
			if (MusicPlayer.IsStopped)
				return;

			ChangePlayingState();
			MediaPlayer.Stop();
			MusicPlayer.IsStopped = true;
		}

		private void Mute(object sender, RoutedEventArgs e)
		{
			VolumeSlider.Value = 0;
			MediaPlayer.Volume = 0;
		}
	}
}