using System;
using PagnisTokens.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new TabbedMain();
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
