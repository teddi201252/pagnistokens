using MySqlConnector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedMain : TabbedPage
    {
        public TabbedMain()
        {
            InitializeComponent();

            MySqlConnection connection = new MySqlConnection("Server=remotemysql.com;Port=3306;Database=7ZZfKCRq3R;Uid=7ZZfKCRq3R;Pwd=KIPGq6QV7W;");
            connection.Open();
        }
    }
}
