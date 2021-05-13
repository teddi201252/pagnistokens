using MySqlConnector;
using PagnisTokens.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
		private NotificationSystem notificationSystem;

		string sqlText;
		MySqlCommand cmd;

		public RegisterPage()
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			notificationSystem = new NotificationSystem();
			AbsoluteRoot.Children.Add(notificationSystem, new Rectangle(0,0,1,1), AbsoluteLayoutFlags.All);
		}

		private void RegisterClicked(object sender, EventArgs e)
		{
			if (UserEntry.Text == null || UserEntry.Text.Trim() == "") 
			{
				notificationSystem.AddNewNotification("Errore", "Non hai inserito un username", Color.Red);
				return;
			}
			else if (UserEntry.Text.Length > 16)
			{
				notificationSystem.AddNewNotification("Errore", "L'username è troppo lungo", Color.Red);
				return;
			}
			else if (PassEntry.Text == null || PassEntry.Text.Trim() == "")
			{
				notificationSystem.AddNewNotification("Errore", "Non hai inserito una password", Color.Red);
				return;
			}
			else if (PassEntry2.Text == null || PassEntry2.Text.Trim() == "")
			{
				notificationSystem.AddNewNotification("Errore", "Non hai confermato la password", Color.Red);
				return;
			}
			else if (PassEntry.Text != PassEntry2.Text)
			{
				notificationSystem.AddNewNotification("Errore", "Le due password non corrispondono", Color.Red);
				return;
			}

			//Tutto in regola
			notificationSystem.AddNewNotification("Invio", "Attendi un attimo...", Color.Orange);

			//Controlla che l'username non esista già
			sqlText = "SELECT username FROM Users WHERE username = @user";
			cmd = new MySqlCommand(sqlText, App.Connection);
			cmd.Parameters.AddWithValue("@user", UserEntry.Text);
			cmd.Prepare();
			MySqlDataReader reader = cmd.ExecuteReader();
			bool found = false;
			while (reader.Read())
			{
				found = true;
			}
			reader.Close();
			if (found)
			{
				notificationSystem.AddNewNotification("Errore", "L'username è già in uso, scegline un altro", Color.Red);
				return;
			}

			sqlText = "INSERT INTO Users (username, password, walletid) VALUES (@user,@pass,@walletid)";
			cmd = new MySqlCommand(sqlText, App.Connection);
			cmd.Parameters.AddWithValue("@user", UserEntry.Text);
			cmd.Parameters.AddWithValue("@pass", UtilFunctions.GetHashedText(PassEntry.Text));
			cmd.Parameters.AddWithValue("@walletid", UtilFunctions.GetHashedText(UserEntry.Text));
			cmd.Prepare();
			cmd.ExecuteNonQuery();
			long idForse = cmd.LastInsertedId;

			sqlText = "INSERT INTO Wallets (id, balance) VALUES (@walletid, 100)";
			cmd = new MySqlCommand(sqlText, App.Connection);
			cmd.Parameters.AddWithValue("@walletid", UtilFunctions.GetHashedText(UserEntry.Text));
			cmd.Prepare();
			cmd.ExecuteNonQuery();

			sqlText = "INSERT INTO Notifications (idUser, title, message) VALUES (@idUser, 'Premio!', 'Hai ricevuto 100 token!')";
			cmd = new MySqlCommand(sqlText, App.Connection);
			cmd.Parameters.AddWithValue("@idUser", idForse);
			cmd.Prepare();
			cmd.ExecuteNonQuery();

			notificationSystem.AddNewNotification("Conferma", "Account creato con successo, torna al login", Color.LightGreen);
		}

		private void Login(object sender, EventArgs args)
		{
			Navigation.InsertPageBefore(new LoginPage(), this);
			Navigation.PopToRootAsync();
			Navigation.RemovePage(this);
		}
	}
}