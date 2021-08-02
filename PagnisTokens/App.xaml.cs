using System;
using FormsControls.Base;
using MySqlConnector;
using PagnisTokens.Utilities;
using PagnisTokens.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens
{
    public partial class App : Application
    {
        private static ApiHelper _apiHelper;
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
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                MainPage = new AnimationNavigationPage(new LoginPage());
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
    }
}
