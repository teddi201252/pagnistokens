using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MySqlConnector;
using PagnisTokens.Utilities;
using Xamarin.Essentials;
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
			if (App.Connection != null)
            {
				//Prova a loggare se si è già loggati una volta
				if (Application.Current.Properties.ContainsKey("username") && Application.Current.Properties.ContainsKey("password"))
				{
					//Fai roba
					sqlText = "SELECT * FROM Users WHERE username = @user AND password = @pass";
					cmd = new MySqlCommand(sqlText, App.Connection);
					cmd.Parameters.AddWithValue("@user", Application.Current.Properties["username"]);
					cmd.Parameters.AddWithValue("@pass", UtilFunctions.GetHashedText(Application.Current.Properties["password"].ToString()));
					cmd.Prepare();
					MySqlDataReader reader = cmd.ExecuteReader();
					bool found = false;
					while (reader.Read())
					{
						found = true;
						Application.Current.Properties["id"] = reader.GetValue(0);
						Application.Current.Properties["walletid"] = reader.GetValue(3);
					}
					reader.Close();

					if (found)
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
            else
            {
				await WaitForLoadedPage();
				notificationSystem.AddNewNotification("Non connesso ad internet", "Controlla di essere connesso e riavvia l'app", Color.Red);
			}
		}

		private async Task WaitForLoadedPage()
        {
            while (App.Current.MainPage.Width == -1)
            {
				await Task.Delay(5);
            }
        }

		private void LoginClicked(System.Object sender, System.EventArgs e)
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

			sqlText = "SELECT * FROM Users WHERE username = @user AND password = @pass";
			cmd = new MySqlCommand(sqlText, App.Connection);
			cmd.Parameters.AddWithValue("@user", UserEntry.Text);
			cmd.Parameters.AddWithValue("@pass", UtilFunctions.GetHashedText(PassEntry.Text));
			cmd.Prepare();
			MySqlDataReader reader = cmd.ExecuteReader();
			bool found = false;
			while (reader.Read())
			{
				found = true;
				Application.Current.Properties["id"] = reader.GetValue(0);
				Application.Current.Properties["walletid"] = reader.GetValue(3);
			}
			reader.Close();

			if (found)
			{
				Application.Current.Properties["username"] = UserEntry.Text;
				Application.Current.Properties["password"] = PassEntry.Text;
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