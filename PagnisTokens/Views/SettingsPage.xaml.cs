using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			InitializeComponent();
		}

		private void Logout(object sender, EventArgs e)
		{
			Application.Current.Properties.Remove("username");
			Application.Current.Properties.Remove("password");
			Application.Current.Properties.Remove("id");
			Application.Current.Properties.Remove("walletid");
			Navigation.InsertPageBefore(new LoginPage(), this.Parent as TabbedPage);
			Navigation.PopToRootAsync();
			Navigation.RemovePage(this.Parent as TabbedPage);

		}
	}
}