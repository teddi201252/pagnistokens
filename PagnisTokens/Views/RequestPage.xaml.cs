using System;
using System.Collections.Generic;
using FormsControls.Base;
using PagnisTokens.Utilities;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace PagnisTokens.Views
{
    public partial class RequestPage : ContentPage, IAnimationPage
    {
        public IPageAnimation PageAnimation { get; } = new SlidePageAnimation { Duration = AnimationDuration.Medium, Subtype = AnimationSubtype.FromTop };

        private NotificationSystem notificationSystem;

        public RequestPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            notificationSystem = new NotificationSystem();
            AbsoluteRoot.Children.Add(notificationSystem, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
        }

        public void OnAnimationFinished(bool isPopAnimation) { }

        public void OnAnimationStarted(bool isPopAnimation) { }

        async void StartTransiction(System.Object sender, System.EventArgs e)
        {
            //Effettua pagamento
            if (EntryImporto.Text == null || EntryImporto.Text.Trim() == "")
            {
                notificationSystem.AddNewNotification("Errore", "Inserisci l'importo da inviare", Color.Red);
                return;
            }
            else if (EntryWallet.Text == null || EntryWallet.Text.Trim() == "")
            {
                notificationSystem.AddNewNotification("Errore", "Inserisci il Wallet ID a cui vuoi inviare il denaro", Color.Red);
                return;
            }
            else if (double.Parse(EntryImporto.Text) - Math.Truncate(double.Parse(EntryImporto.Text)) != 0 && double.Parse(EntryImporto.Text) - Math.Truncate(double.Parse(EntryImporto.Text)) < 0.000001)
            {
                notificationSystem.AddNewNotification("Errore", "Il valore decimale dell'importo è troppo grande (massimo 6 cifre)", Color.Red);
                return;
			}
			else if (double.Parse(EntryImporto.Text) < 0)
			{
                notificationSystem.AddNewNotification("Errore", "Inserisci un numero positivo", Color.Red);
                return;
			}
			else if (EntryWallet.Text == Application.Current.Properties["walletid"].ToString())
			{
                notificationSystem.AddNewNotification("Errore", "Non puoi inviarti soldi da solo! Bruh", Color.Red);
                return;
            }

            //Controlla se il walletId inserito esiste
            string idUtenteDaPagare = (await App.apiHelper.getUserByWalletId(EntryWallet.Text))?.id;
            if (idUtenteDaPagare == null)
            {
                notificationSystem.AddNewNotification("Errore", "Wallet ID non esistente", Color.Red);
                return;
            }

            App.apiHelper.paySomeone(double.Parse(EntryImporto.Text), App.Current.Properties["walletid"].ToString(), EntryWallet.Text);

            await Navigation.PopAsync();
        }

        async void ScanQRCode(System.Object sender, System.EventArgs e)
        {
            try
            {
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
                    TryHarder = true
                };

                var overlay = new ZXingDefaultOverlay
                {
                    TopText = "Scannerizza il codice QR"
                };

                var QRScanner = new ZXingScannerPage(options, overlay);

                await Navigation.PushModalAsync(QRScanner);

                QRScanner.OnScanResult += (result) =>
                {
                    // Stop scanning
                    QRScanner.IsScanning = false;

                    // Pop the page and show the result
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        EntryWallet.Text = result.Text;
                    });

                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
    }
}
