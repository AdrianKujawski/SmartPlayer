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

		public async void SignIn(object sender, RoutedEventArgs e) {
			InfoPanel.Visibility = Visibility;
#if DEBUG
			LoginBox.Text = "AdrianXIX";
			PasswordBox.Text = "123qwe";
#endif
			if (!CheckTextBox()) {
				ErrorBlock.Text = "Źle wpisany login lub haslo";
				return;
			}

			var user = new User(LoginBox.Text, PasswordBox.Text);
			var isCorrect = await CheckLoginAndPassword(user);
			if (!isCorrect) return;

			MusicPlayer.ActualUser = user;
			Window.Current.Content = new MainPage();
		}

		bool CheckTextBox() {
			return !string.IsNullOrWhiteSpace(LoginBox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Text);
		}

		async Task<bool> CheckLoginAndPassword(User user) {
			var isCorrect = false;
			try {
				isCorrect = await user.CheckLoginAndPassword();
			}
			catch (Exception exc) {
				var msg = new MessageDialog(exc.Message + "/n" + exc.InnerException + "/n" + exc.Source);
				await msg.ShowAsync();
				ErrorBlock.Text = exc.Message;
			}
			return isCorrect;
		}
	}

}
