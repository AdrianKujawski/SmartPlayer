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
		public LoginPage() {
			InitializeComponent();
		}

		async void SignIn(object sender, RoutedEventArgs e) {
			DisableControllers();
#if DEBUG
			LoginBox.Text = "AdrianXIX";
			PasswordBox.Password = "123qwe";
#endif
			if (!CheckTextBox()) {
				InfoPanel.Visibility = Visibility;
				ErrorBlock.Text = "Źle wpisane dane.";
				return;
			}

			var user = new User(LoginBox.Text, PasswordBox.Password);
			var isCorrect = await CheckLoginAndPassword(user);
			if (!isCorrect) {
				InfoPanel.Visibility = Visibility;
				ErrorBlock.Text = "Nieporawny login lub hasło.";
				return;
			}

			MusicPlayer.ActualUser = user;
			Window.Current.Content = new MainPage();
		}

		bool CheckTextBox() {
			return !string.IsNullOrWhiteSpace(LoginBox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Password);
		}

		async Task<bool> CheckLoginAndPassword(User user) {
			var isCorrect = false;
			try {
				isCorrect = await user.CheckLoginAndPassword();
			}
			catch (Exception exc) {
				ErrorBlock.Text = "Nie można połączyć się z serwerem.";
			}
			return isCorrect;
		}

		void DisableControllers() {
			LoignButton.IsEnabled = false;
			LoginBox.IsEnabled = false;
			PasswordBox.IsEnabled = false;
		}
	}

}
