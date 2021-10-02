using MySqlConnector;
using PagnisTokens.Models;
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

		private async void RegisterClicked(object sender, EventArgs e)
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
			else if (UserEntry.Text.Length < 4)
			{
				notificationSystem.AddNewNotification("Errore", "L'username è troppo corto", Color.Red);
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
			List<UserModel> listSameUser = await App.apiHelper.searchUsersByUsername(UserEntry.Text);
			if (listSameUser != null)
			{
				foreach (var possibleUser in listSameUser)
				{
					if (possibleUser.username == UserEntry.Text)
					{
						notificationSystem.AddNewNotification("Errore", "L'username è già in uso, scegline un altro", Color.Red);
						return;
					}
				}
			}

			bool successfulRegistration = await App.apiHelper.tryRegister(UserEntry.Text, PassEntry.Text);
			if (successfulRegistration)
			{
				notificationSystem.AddNewNotification("Conferma", "Account creato con successo, torna al login", Color.LightGreen);
			}
			else
			{
				notificationSystem.AddNewNotification("Registrazione fallita", "Account non creato, riprova più tardi", Color.Red);
			}

			
		}

		private void Login(object sender, EventArgs args)
		{
			Navigation.InsertPageBefore(new LoginPage(), this);
			Navigation.PopToRootAsync();
			Navigation.RemovePage(this);
		}
	}
}