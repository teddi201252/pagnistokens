using MySqlConnector;
using PagnisTokens.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			//Prova a loggare se si è già loggati una volta
			if (Application.Current.Properties.ContainsKey("username") && Application.Current.Properties.ContainsKey("password"))
			{
				//Fai roba
			}


			//string sqlText = "INSERT INTO Users (nickname, password, walletid) VALUES (@user,@pass,@walletid)";
			//MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
			//cmd.Parameters.AddWithValue("@user", "Utente");
			//cmd.Parameters.AddWithValue("@pass", UtilFunctions.GetHashedText("Password"));
			//cmd.Parameters.AddWithValue("@walletid", "questoeilwalletid12345");
			//cmd.ExecuteNonQuery();
		}
	}
}