using System;
using FormsControls.Base;
using PagnisTokens.Utilities;
using PagnisTokens.Views;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace PagnisTokens
{
    public partial class App : Application
    {
        private static ApiHelper _apiHelper;
        public static bool isConnected = true;
        public static ApiHelper apiHelper { get 
            {
				if (_apiHelper == null)
				{
                    _apiHelper = new ApiHelper();
                }
                return _apiHelper; 
            }
        }

		public App()
        {
            InitializeComponent();
            if (CrossConnectivity.Current.IsConnected)
            {
				Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    return CheckConnection();
                });
                AnimationNavigationPage amo = new AnimationNavigationPage(new LoginPage());
                MainPage = amo;
			}
            
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private bool CheckConnection()
        {
            if (isConnected == true && !CrossConnectivity.Current.IsConnected)
            {
                isConnected = false;
                new NotificationSystem().AddNewNotification("Non connesso ad internet", "Controlla di essere connesso e riavvia l'app", Color.Red);
                return false;
            }
            else
                return true;
        }
    }
}
