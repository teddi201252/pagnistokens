using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MySqlConnector;
using PagnisTokens.Models;
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
            byte[] qrCodeBytes = qrCode.GetGraphic(20, "#000000", "#ffffff");
            
            ImageSource QrCodeImageSource = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
            QrCodeImage.Source = QrCodeImageSource;

            WalletIdLabel.Text = Application.Current.Properties["walletid"].ToString();

        }

        System.Threading.Timer timer = null;
        protected override void OnAppearing()
		{
			base.OnAppearing();
            LoadBalance();
            LoadNotifications();

            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(100);

            if (timer == null)
            {
                timer = new System.Threading.Timer((e) =>
                {
                    if (((TabbedPage)this.Parent).CurrentPage == this)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            LoadBalance();
                            LoadNotifications();
                        });
                    }
                }, null, startTimeSpan, periodTimeSpan);
            }
            
        }


        private async void LoadBalance()
        {
			if (!Application.Current.Properties.ContainsKey("walletid"))
			{
                return;
			}
            
            BalanceLabel.Text = await App.apiHelper.getBalanceFromWallet(Application.Current.Properties["walletid"].ToString());
                
        }

        private async void LoadNotifications()
        {
            if (!Application.Current.Properties.ContainsKey("id"))
            {
                return;
            }
            List<NotificationModel> notes = await App.apiHelper.getNotificationsOfUser(Application.Current.Properties["id"].ToString());
			foreach (var nota in notes)
			{
				if (!nota.seen)
				{
                    notificationSystem.AddNewNotification(nota.title, nota.message, Xamarin.Forms.Color.White);
                    App.apiHelper.updateNotificationById(nota.id);
                }
			}
        }

		private async void CopyWalletId(object sender, EventArgs e)
		{
            await Clipboard.SetTextAsync(WalletIdLabel.Text);
            notificationSystem.AddNewNotification("Fatto", "Wallet ID copiato negli appunti", Xamarin.Forms.Color.LightGreen);
		}

        void IniziaPagamento(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new PaymentPage(), true);
        }

        void RichiediPagamento(System.Object sender, System.EventArgs e)
        {
        }

        void ApriNotifiche(System.Object sender, System.EventArgs e)
		{
            Navigation.PushAsync(new NotificationsPage(), true);
        }
    }
}
