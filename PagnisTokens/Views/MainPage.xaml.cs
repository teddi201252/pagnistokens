using System;
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
            string sqlText = "SELECT balance FROM Wallets WHERE id = @walletid";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@walletid", Application.Current.Properties["walletid"].ToString());
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
                BalanceLabel.Text = UtilFunctions.FormatBalance(reader.GetDouble(0));
			}
            reader.Close();
		}

		private async void CopyWalletId(object sender, EventArgs e)
		{
            await Clipboard.SetTextAsync(WalletIdLabel.Text);
            notificationSystem.AddNewNotification("Fatto", "Wallet ID copiato negli appunti", Xamarin.Forms.Color.LightGreen);
		}
	}
}
