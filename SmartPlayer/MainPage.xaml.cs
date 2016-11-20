// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Adrian Kujawski">
//     Copyright (c) 2016, Adrian Kujawski. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using SmartPlayer.Controller;
using SmartPlayer.Lists;
using SmartPlayer.Model;
using SmartPlayer.NotifyFolder;
using SmartPlayer.Relation;
using SmartPlayer.Song;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartPlayer {

	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage {
		readonly double _songDurationPart = 30;
		bool _isUpdated;

		public MainPage() {
			InitializeComponent();
			VolumeSlider.Value = 100;
			ListViewSongs.Tapped += SelectSongFromPlaylist;
			SongName.Text = "";
			_isUpdated = false;
			StatusText.Text = "Witaj " + MusicPlayer.ActualUser.GetName();
			TimeTask.DispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
			SetTimeTask();
			Notify.TextBlock = StatusText;
			Notify.Border = StatusBorder;
			PlayerController.MediaPlayer = MediaPlayer;
		}

		async void OpenFiles(object sender, RoutedEventArgs e) {
			var fileOpener = new FileOpener();
			var files = await fileOpener.Open();

			if (files != null) {
				Playlist.AddSongToPlaylist(files);
				ListViewSongs.ItemsSource = Playlist.GetListOfSongs();
			}
		}

		void SongOpened(object sender, RoutedEventArgs e) {
			SetDurationSong();
			TimelineSlider.Value = 0;
			TimelineSlider.Maximum = SongTime.DuractionSong;
			MediaPlayer.Volume = VolumeSlider.Value;

			if (MediaPlayer.CurrentState == MediaElementState.Paused || MediaPlayer.CurrentState == MediaElementState.Stopped)
				PlaySong();
		}

		void SongEnded(object sender, RoutedEventArgs e) {
			NextSong(1);
			StartSongAgain();
		}

		void StartSongAgain() {
			Time.Text = "00:00:00";
			TimelineSlider.Value = 0;
			SongTime.ActualSecond = 0;
		}

		void SetDurationSong() {
			_isUpdated = false;
			SongTime.ActualSecond = 0;
			SongTime.DuractionSong = (int)MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
			SongTime.TimeToExamine = Math.Floor(SongTime.DuractionSong / _songDurationPart);
		}

		void SetTimeTask() {
			TimeTask.AddMethod(SetCurrentSongTime);
			TimeTask.AddMethod(ChangeSlidePosition);
			TimeTask.AddMethod(WaitToListenTo);
		}

		async void WaitToListenTo(object sender, object e) {
#if DEBUG
			Debug.WriteLine("T: {0} >= {1}", SongTime.ActualSecond, SongTime.TimeToExamine);
#endif
			if ((SongTime.ActualSecond >= SongTime.TimeToExamine) && !_isUpdated) {
				_isUpdated = true;
				try {
					var isExist = await RelationUpdater.IsRelationExist();
					if (!isExist)
						await CreateRelation.AddUserSongRelation();
					await RelationUpdater.UpdateSongQty();
					Notify.SetNotify("Piosenka przesluchana!", NotifyType.StatusMessage);
				}
				catch (Exception) {
					Notify.SetNotify("Błąd podczas aktualizacji danych.", NotifyType.ErrorMessage);
				}
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

		async void SelectSongFromPlaylist(object sender, TappedRoutedEventArgs tappedRoutedEventArgs) {
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

		static async void CheckSongInDataBase(SongFile song) {
			var isSongExist = await Service.IsSongExist(song.Title, song.Album, song.Artist);

			if (isSongExist)
				Notify.SetNotify(song.Title + " - istnieje.", NotifyType.StatusMessage);
			else {
				Notify.SetNotify("Piosenka zostanie dodana do bazy...", NotifyType.ErrorMessage);
				var result = await SongInsert.AddSong(song);
				if(result) Notify.SetNotify(song.Title + " - dodano do bazy.", NotifyType.StatusMessage);

			}
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
			PlayerController.PauseSong();
			PlayPauseButton.Content = "►";
		}

		void PlaySong() {
			PlayPauseButton.Content = "||";
			PlayerController.PlaySong();

			if (MediaPlayer.CurrentState == MediaElementState.Stopped)
				StartSongAgain();
		}

		void StopSong(object sender, RoutedEventArgs e) {
			if (MediaPlayer.CurrentState == MediaElementState.Stopped || MediaPlayer.CurrentState != MediaElementState.Playing)
				return;

			PlayerController.StopSong();
			PlayPauseButton.Content = "►";
			Time.Text = "00:00:00";
			TimelineSlider.Value = 0;
		}

		async void NextSong(int sign) {
			if (ListViewSongs.Items != null && ListViewSongs.Items.Count == 0) return;

			if (ListViewSongs.Items != null) {
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
			}

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
			if (!MediaPlayer.IsMuted) {
				MusicPlayer.TempVolumeSlider = VolumeSlider.Value;
				MusicPlayer.TempVolumeValue = MediaPlayer.Volume;
				VolumeSlider.Value = 0;
				MediaPlayer.Volume = 0;
			}
			else {
				VolumeSlider.Value = MusicPlayer.TempVolumeSlider;
				MediaPlayer.Volume = MusicPlayer.TempVolumeValue;
			}

			MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
		}

		async void ShowStatistics(object sender, RoutedEventArgs e) {
			var isPlayerActiv = PlayerPanel.Visibility == Visibility.Visible;

			if (isPlayerActiv) {
				PlayerPanel.Visibility = Visibility.Collapsed;
				StatisticPanel.Visibility = Visibility.Visible;
				
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

		void ClearPlaylist(object sender, RoutedEventArgs e) {
			TimeTask.Stop();
			MediaPlayer.Stop();
			MusicPlayer.ActualSong = null;
			PlayPauseButton.Content = "||";
			ListViewSongs.ItemsSource = null;
			Playlist.Clear();
		}

		static void DispatcherClear() {
			if (TimeTask.DispatcherTimer == null) return;

			TimeTask.Stop();
			TimeTask.Disable();
		}

		async void DropSongsToPlaylist(object sender, DragEventArgs e) {
			var files = await e.DataView.GetStorageItemsAsync();
			var listOfFiles = files.Cast<StorageFile>().ToList();
			Playlist.AddSongToPlaylist(listOfFiles);
			ListViewSongs.ItemsSource = Playlist.GetListOfSongs();
		}

		void SetAcceptedOperation(object sender, DragEventArgs e) {
			e.AcceptedOperation = DataPackageOperation.Copy;
		}

		async void ShowAlbumStatistic(object sender, RoutedEventArgs e) {
			await AlbumStatisticList.AddSongs();
			StatisticListSong.ItemsSource = SongStatisticList.GetList();

		}

		async void ShowArtistStatistic(object sender, RoutedEventArgs e) {
			await ArtistStatisticList.AddSongs();
			StatisticListSong.ItemsSource = SongStatisticList.GetList();
		}

		async void ShowTitleStatistic(object sender, RoutedEventArgs e) {
			await SongStatisticList.AddSongs();
			StatisticListSong.ItemsSource = SongStatisticList.GetList();
		}
	}

}
