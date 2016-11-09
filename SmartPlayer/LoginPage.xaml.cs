using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SmartPlayer.Controller;
using SmartPlayer.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartPlayer
{
	/// <summary>
	///     An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page
	{
		public LoginPage()
		{
			InitializeComponent();
		}

		public async void SignIn(object sender, RoutedEventArgs e)
		{
			InfoPanel.Visibility = Visibility;
#if DEBUG
			LoginBox.Text = "AdrianXIX";
			PasswordBox.Text = "123qwe";
#endif
			if (!CheckTextBox())
			{
				ErrorBlock.Text = "Źle wpisany login lub haslo";
				return;
			}

			var user = new User(LoginBox.Text, PasswordBox.Text);
			var isCorrect = await user.CheckLoginAndPassword();
			if (isCorrect)
			{
				MusicPlayer.ActualUser = user;
				Window.Current.Content = new MainPage();
			}
			else
				ErrorBlock.Text = "Niepoprawny login lub hasło";
		}

		private bool CheckTextBox()
		{
			return !string.IsNullOrWhiteSpace(LoginBox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Text);
		}
	}
}