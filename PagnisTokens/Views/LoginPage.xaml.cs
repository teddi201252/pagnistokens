using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MySqlConnector;
using PagnisTokens.Models;
using PagnisTokens.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : INotifyPropertyChanged
	{
		private NotificationSystem notificationSystem;

        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isLoaded = false;
		public bool IsLoaded
		{
			get { return _isLoaded; }

			set
			{
				_isLoaded = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoaded)));
			}
		}

		string sqlText;
		MySqlCommand cmd;
		public LoginPage()
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			notificationSystem = new NotificationSystem();
			AbsoluteRoot.Children.Add(notificationSystem, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
		}

        protected override async void OnAppearing()
        {
            base.OnAppearing();
			if (App.isConnected)
            {
				//Prova a loggare se si è già loggati una volta
				if (Application.Current.Properties.ContainsKey("username") && Application.Current.Properties.ContainsKey("password"))
				{
					//Prova a fare il login
					UserModel userFound = await App.apiHelper.tryLogin(Application.Current.Properties["username"].ToString(), Application.Current.Properties["password"].ToString());

					if (userFound != null)
					{
						LoggedIn();
                    }
                    else
					{
						Application.Current.Properties.Remove("username");
						Application.Current.Properties.Remove("password");
						Application.Current.Properties.Remove("id");
						Application.Current.Properties.Remove("walletid");
					}
				}
            }
		}

		private async Task WaitForLoadedPage()
        {
            while (App.Current.MainPage.Width == -1)
            {
				await Task.Delay(5);
            }
        }

		private async void LoginClicked(System.Object sender, System.EventArgs e)
        {
			if (UserEntry.Text == null || UserEntry.Text.Trim() == "")
			{
				notificationSystem.AddNewNotification("Errore", "Non hai inserito l'username", Color.Red);
				return;
			}
			else if (PassEntry.Text == null || PassEntry.Text.Trim() == "")
			{
				notificationSystem.AddNewNotification("Errore", "Non hai inserito la password", Color.Red);
				return;
			}

			UserModel userFound = await App.apiHelper.tryLogin(UserEntry.Text, PassEntry.Text);
			if (userFound != null)
			{
				Application.Current.Properties["id"] = userFound.id;
				Application.Current.Properties["username"] = userFound.username;
				Application.Current.Properties["password"] = PassEntry.Text;
				Application.Current.Properties["walletid"] = userFound.walletid;
				LoggedIn();
            }
            else
            {
				PassEntry.Text = "";
				notificationSystem.AddNewNotification("Errore", "Username o password errati", Color.Red);
				return;
			}
		}

		private void LoggedIn()
        {
			Navigation.InsertPageBefore(new TabbedMain(), this);
			Navigation.PopToRootAsync();
			Navigation.RemovePage(this);
		}

		public void Registrati(object sender, EventArgs args)
		{
			Navigation.InsertPageBefore(new RegisterPage(), this);
			Navigation.PopToRootAsync();
			Navigation.RemovePage(this);
		}

	}
}