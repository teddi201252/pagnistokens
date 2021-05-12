using System;
using System.Diagnostics;
using MySqlConnector;
using PagnisTokens.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		string sqlText;
		MySqlCommand cmd;
		public LoginPage()
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

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
			}

            //sqlText = "INSERT INTO Users (username, password, walletid) VALUES (@user,@pass,@walletid)";
            //cmd = new MySqlCommand(sqlText, App.Connection);
            //cmd.Parameters.AddWithValue("@user", "Utente");
            //cmd.Parameters.AddWithValue("@pass", UtilFunctions.GetHashedText("Password"));
            //cmd.Parameters.AddWithValue("@walletid", "questoeilwalletid12345");
            //cmd.ExecuteNonQuery();

        }

		private void LoginClicked(System.Object sender, System.EventArgs e)
        {
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
            }
		}

		private void LoggedIn()
        {
			Navigation.InsertPageBefore(new TabbedMain(), this);
			Navigation.PopToRootAsync();
			Navigation.RemovePage(this);
		}
	}
}