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
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			//Prova a loggare se si è già loggati una volta
			if (Application.Current.Properties.ContainsKey("username") && Application.Current.Properties.ContainsKey("password"))
			{

			}
		}
	}
}