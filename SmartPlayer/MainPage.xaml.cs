// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Unicore">
//     Copyright (c) 2016, Unicore. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using SmartPlayer.Controller;
using SmartPlayer.Lists;
using SmartPlayer.Model;
using SmartPlayer.Relation;
using SmartPlayer.Song;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartPlayer {

	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page {
		public enum NotifyType {
			StatusMessage,
			ErrorMessage
		}

		TimeTask _timeTask;

		readonly double _songDurationPart = 30;
		double _halfDuractionSong;
		bool _isUpdated;
		ScrollViewer scollViewer;

		public MainPage() {
			this.scollViewer = scollViewer;
			InitializeComponent();
			VolumeSlider.Value = 100;
			ListViewSongs.Tapped += mediaPlayer_ItemClick;
			SongName.Text = "";
			_isUpdated = false;
			StatusText.Text = "Witaj " + MusicPlayer.ActualUser.GetName();
			SetTimeTask();
		}


		async void OpenFiles(object sender, RoutedEventArgs e) {
			var fileOpener = new FileOpener();
			var files = await fileOpener.Open();

			if (files != null) {
				Playlist.AddSongToPlaylist(files);
				ListViewSongs.ItemsSource = Playlist.GetListOfSongs();
			}
		}

		void songOpened(object sender, RoutedEventArgs e) {
			SetDurationSong();
			TimelineSlider.Value = 0;
			TimelineSlider.Maximum = SongTime.DuractionSong;
			MediaPlayer.Volume = VolumeSlider.Value;

			if (MediaPlayer.CurrentState == MediaElementState.Paused || MediaPlayer.CurrentState == MediaElementState.Stopped)
				PlaySong();
		}

		void songEnded(object sender, RoutedEventArgs e) {
			NextSong(1);
			StartSongAgain();
		}

		void StartSongAgain() {
			Time.Text = "00:00:00";
			TimelineSlider.Value = 0;
			SetDurationSong();
			MediaPlayer.Play();
		}

		void SetDurationSong() {
			_isUpdated = false;
			SongTime.ActualSecond = 0;
			SongTime.DuractionSong = (int)MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
			SongTime.TimeToExamine = Math.Floor(SongTime.DuractionSong / _songDurationPart);
		}

		void SetTimeTask() {
			_timeTask = new TimeTask();
			_timeTask.AddMethod(SetCurrentSongTime);
			_timeTask.AddMethod(ChangeSlidePosition);
			_timeTask.AddMethod(WaitForListningSong);
		}

		async void WaitForListningSong(object sender, object e) {
#if DEBUG
			Debug.WriteLine("T: {0} >= {1}", SongTime.ActualSecond, SongTime.TimeToExamine);
#endif
			if ((SongTime.ActualSecond >= SongTime.TimeToExamine) && !_isUpdated) {
				_isUpdated = true;
				var isExist = await RelationUpdater.IsRelationExist();
				if (!isExist)
					await CreateRelation.AddUserSongRelation();
				await RelationUpdater.UpdateSongQty();
				NotifyUser("Piosenka przesluchana!", NotifyType.StatusMessage);
			}
			SongTime.ActualSecond++;
		}

		void ChangeSlidePosition(object sender, object e) {
			TimelineSlider.Value += 1;
		}

		void SetCurrentSongTime(object sender, object e) {
			ChangeTimeTextValue();
		}

		void SeekToMediaPosition(object sender, PointerRoutedEventArgs pointerRoutedEventArgs) {
			var sliderValue = (int)TimelineSlider.Value;

			var ts = new TimeSpan(0, 0, 0, sliderValue, 0);
			MediaPlayer.Position = ts;
			ChangeTimeTextValue();
		}

		void ChangeTimeTextValue() {
			var songTime = MediaPlayer.Position.ToString(@"hh\:mm\:ss");
			Time.Text = songTime;
		}

		void ChangeMediaVolume(object sender, RangeBaseValueChangedEventArgs e) {
			MediaPlayer.Volume = VolumeSlider.Value;
		}

		async void mediaPlayer_ItemClick(object sender, TappedRoutedEventArgs tappedRoutedEventArgs) {
			var selectedSong = ListViewSongs.SelectedItem as SongFile;
			if (selectedSong == null) {
				ListViewSongs.SelectedItem = null;
				return;
			}

			await SetPlayerSource(selectedSong);
		}

		async Task SetPlayerSource(SongFile song) {
			MusicPlayer.ActualSong = song;
			MediaPlayer.SetSource(await song.File.OpenAsync(FileAccessMode.Read), song.File.ContentType);
			SongName.Text = song.GetFullName();
			CheckSongInDataBase(song);
		}

		async void CheckSongInDataBase(SongFile song) {
			var isSongExist = await Service.IsSongExist(song.Title, song.Album, song.Artist);

			if (isSongExist)
				NotifyUser(song.Title + " - istnieje.", NotifyType.StatusMessage);
			else {
				NotifyUser("Piosenka zostanie dodana do bazy...", NotifyType.ErrorMessage);
				var adder = new SongInsert(song);
				try {
					adder.AddSong();
					NotifyUser(song.Title + " - dodano do bazy.", NotifyType.StatusMessage);
				}
				catch (Exception) {
					NotifyUser("Coś poszło nie tak...", NotifyType.ErrorMessage);
				}
			}
		}

		public void NotifyUser(string strMessage, NotifyType type) {
			switch (type) {
				case NotifyType.StatusMessage:
					StatusBorder.Background = new SolidColorBrush(Colors.Green);
					break;
				case NotifyType.ErrorMessage:
					StatusBorder.Background = new SolidColorBrush(Colors.Red);
					break;
			}

			StatusText.Text = strMessage;
		}

		void ChangePlayingState(object sender, RoutedEventArgs e) {
			if (ListViewSongs.SelectedItem != null)
				ChangePlayingState();
		}

		void ChangePlayingState() {
			if (MediaPlayer.CurrentState != MediaElementState.Playing)
				PlaySong();
			else
				PauseSong();
		}

		void PauseSong() {
			MediaPlayer.Pause();
			PlayPauseButton.Content = "►";
			_timeTask.Stop();
		}

		void PlaySong() {
			PlayPauseButton.Content = "||";
			if (MediaPlayer.CurrentState != MediaElementState.Stopped) {
				MediaPlayer.Play();
				_timeTask.Start();
			}
			else {
				StartSongAgain();
				_timeTask.Start();
			}
		}

		void StopSong(object sender, RoutedEventArgs e) {
			if (MediaPlayer.CurrentState == MediaElementState.Stopped || MediaPlayer.CurrentState != MediaElementState.Playing)
				return;
			PauseSong();
			MediaPlayer.Stop();
			Time.Text = "00:00:00";
			TimelineSlider.Value = 0;
		}

		async void NextSong(int sign) {
			if (ListViewSongs.Items.Count == 0) return;

			var numberOfSong = ListViewSongs.Items.Count - 1;
			if (numberOfSong == 0)
				return;

			var currentSongPosition = ListViewSongs.SelectedIndex;
			SongFile nextSong;
			if (currentSongPosition < numberOfSong) {
				currentSongPosition += sign;
				if (currentSongPosition < 0)
					currentSongPosition = numberOfSong;
				nextSong = GetNextSong(currentSongPosition);
			}
			else {
				if (sign == 1)
					currentSongPosition = 0;
				else
					currentSongPosition += sign;
				nextSong = GetNextSong(currentSongPosition);
			}

			await SetPlayerSource(nextSong);
			SetScrollViewer();
		}

		void NextSong(object sender, RoutedEventArgs e) {
			var sign = SetDirectionOfChange(sender);
			NextSong(sign);
		}

		SongFile GetNextSong(int position) {
			ListViewSongs.SelectedIndex = position;
			return ListViewSongs.Items.ElementAt(position) as SongFile;
		}

		static int SetDirectionOfChange(object sender) {
			var buttonName = sender as Button;
			return buttonName != null && buttonName.Name == "NextButton" ? 1 : -1;
		}

		void Mute(object sender, RoutedEventArgs e) {
			if (!MusicPlayer.IsMute) {
				MusicPlayer.TempVolumeSlider = VolumeSlider.Value;
				MusicPlayer.TempVolumeValue = MediaPlayer.Volume;
				VolumeSlider.Value = 0;
				MediaPlayer.Volume = 0;
			}
			else {
				VolumeSlider.Value = MusicPlayer.TempVolumeSlider;
				MediaPlayer.Volume = MusicPlayer.TempVolumeValue;
			}

			MusicPlayer.IsMute = !MusicPlayer.IsMute;
		}

		async void StatisticButton_Click(object sender, RoutedEventArgs e) {
			var isPlayerActiv = PlayerPanel.Visibility == Visibility.Visible;

			if (isPlayerActiv) {
				PlayerPanel.Visibility = Visibility.Collapsed;
				StatisticPanel.Visibility = Visibility.Visible;
				await StatisticList.AddSongsToStatisticView();
				StatisticListSong.ItemsSource = StatisticList.GetList();
			}
			else {
				PlayerPanel.Visibility = Visibility.Visible;
				StatisticPanel.Visibility = Visibility.Collapsed;
			}
		}

		void LogoutUser(object sender, RoutedEventArgs e) {
			DispatcherClear();
			MusicPlayer.ActualUser = null;
			Window.Current.Content = new LoginPage();
		}

		void SetScrollViewer() {
			ListViewSongs.ScrollIntoView(MusicPlayer.ActualSong);
		}

		void ClearButton_OnClick(object sender, RoutedEventArgs e) {
			_timeTask.Stop();
			MediaPlayer.Stop();
			MusicPlayer.ActualSong = null;
			PlayPauseButton.Content = "||";
			ListViewSongs.ItemsSource = null;
			Playlist.ClearList();
		}

		void DispatcherClear() {
			if (_timeTask != null) {
				_timeTask.Stop();
				_timeTask = null;
			}
		}
	}

}
