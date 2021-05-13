using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MySqlConnector;
using PagnisTokens.Utilities;
using QRCoder;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PagnisTokens.Views
{
	public partial class MainPage : ContentPage
    {
        NotificationSystem notificationSystem;

        public MainPage()
        {
            InitializeComponent();
            notificationSystem = new NotificationSystem();
            AbsoluteRoot.Children.Add(notificationSystem, new Xamarin.Forms.Rectangle(0,0,1,1), AbsoluteLayoutFlags.All);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Application.Current.Properties["walletid"].ToString(), QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20, "#6b6b6b", "#ededed");
            
            ImageSource QrCodeImageSource = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
            QrCodeImage.Source = QrCodeImageSource;

            WalletIdLabel.Text = Application.Current.Properties["walletid"].ToString();
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();
            LoadBalance();
            LoadNotifications();
		}

        private void LoadBalance()
        {
            string sqlText = "SELECT balance FROM Wallets WHERE id = @walletid";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@walletid", Application.Current.Properties["walletid"]);
            cmd.Prepare();
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    BalanceLabel.Text = UtilFunctions.FormatBalance(reader.GetDouble(0));
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("ERRORE: Impossibile leggere i dati dal database");
            }
        }

        private void LoadNotifications()
        {
            Console.WriteLine(Application.Current.Properties["id"]);
            string sqlText = "SELECT * FROM Notifications WHERE idUser = @idUser";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idUser", Application.Current.Properties["id"]);
            cmd.Prepare();
            try
            {
                List<int> notificheViste = new List<int>();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.GetBoolean("seen"))
                    {
                        notificationSystem.AddNewNotification(reader.GetString("title"), reader.GetString("message"), Xamarin.Forms.Color.White);
                        notificheViste.Add(reader.GetInt16("id"));
                    }
                }
                reader.Close();

                sqlText = "UPDATE Notifications SET seen=true WHERE id = @idNot";
                cmd = new MySqlCommand(sqlText, App.Connection);
                foreach (int idNot in notificheViste)
                {
                    cmd.Parameters.AddWithValue("@idNot", idNot);
                    cmd.ExecuteNonQuery();
                }
                
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("ERRORE: Impossibile leggere i dati dal database");
            }
        }

		private async void CopyWalletId(object sender, EventArgs e)
		{
            await Clipboard.SetTextAsync(WalletIdLabel.Text);
            notificationSystem.AddNewNotification("Fatto", "Wallet ID copiato negli appunti", Xamarin.Forms.Color.LightGreen);
		}
	}
}
