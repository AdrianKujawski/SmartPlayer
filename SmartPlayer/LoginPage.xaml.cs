// -----------------------------------------------------------------------
// <copyright file="LoginPage.xaml.cs" company="Unicore">
//     Copyright (c) 2016, Unicore. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SmartPlayer.Controller;
using SmartPlayer.Model;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartPlayer {

	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page {

		User _user;

		public LoginPage() {
			InitializeComponent();
		}

		async void SignIn(object sender, RoutedEventArgs e) {
			var result = await TryToSignIn();
			if (!result) {
				SwitchControllers();
				InfoPanel.Visibility = Visibility;
			}
			else {
				MusicPlayer.ActualUser = _user;
				Window.Current.Content = new MainPage();
			}
		}

		async Task<bool> TryToSignIn() {
			SwitchControllers();
#if DEBUG
			LoginBox.Text = "AdrianXIX";
			PasswordBox.Password = "123qwe";
#endif
			if (!CheckTextBox()) {
				ErrorBlock.Text = "Źle wpisane dane.";
				return false;
			}

			_user = new User(LoginBox.Text, PasswordBox.Password);
			var isCorrect = await CheckLoginAndPassword(_user);
			if (isCorrect) return true;

			ErrorBlock.Text = "Nieporawny login lub hasło.";
			return false;
		}

		bool CheckTextBox() {
			return !string.IsNullOrWhiteSpace(LoginBox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Password);
		}

		async Task<bool> CheckLoginAndPassword(User user) {
			var isCorrect = false;
			try {
				isCorrect = await user.CheckLoginAndPassword();
			}
			catch (Exception) {
				ErrorBlock.Text = "Nie można połączyć się z serwerem.";
			}
			return isCorrect;
		}

		void SwitchControllers() {
			LoignButton.IsEnabled = !LoignButton.IsEnabled;
			LoginBox.IsEnabled = !LoginBox.IsEnabled;
			PasswordBox.IsEnabled = !PasswordBox.IsEnabled;
		}
	}

}
