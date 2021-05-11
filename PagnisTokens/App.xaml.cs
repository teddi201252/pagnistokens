using System;
using MySqlConnector;
using PagnisTokens.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens
{
    public partial class App : Application
    {
        private MySqlConnection _connection;
        public MySqlConnection Connection { get { return _connection; } }
        
        public App()
        {
            InitializeComponent();
            _connection = new MySqlConnection("Server=remotemysql.com;Port=3306;Database=7ZZfKCRq3R;Uid=7ZZfKCRq3R;Pwd=KIPGq6QV7W;");
            _connection.Open();
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
