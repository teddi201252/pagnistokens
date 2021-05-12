using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PagnisTokens.Utilities
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationSystem : ContentView
	{
		List<Frame> listaNotifiche = new List<Frame>();
		public NotificationSystem()
		{
			InitializeComponent();
		}

		public void AddNewNotification(string title, string message, Color color)
		{
			StackLayout stackLabel = new StackLayout
			{
				Padding = 0,
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			Label titoloLabel = new Label
			{
				Margin = 0,
				Text = title,
				TextColor = UtilFunctions.ReverseColor(color),
				FontAttributes = FontAttributes.Bold
			};
			stackLabel.Children.Add(titoloLabel);
			Label msgLabel = new Label
			{
				Margin = 0,
				Text = message,
				TextColor = UtilFunctions.ReverseColor(color)
			};
			stackLabel.Children.Add(msgLabel);
			Frame notifica = new Frame
			{
				BackgroundColor = color,
				CornerRadius = 10,
				Content = stackLabel
			};
			AbsoluteRoot.Children.Add(notifica, new Rectangle(0.5, -(App.Current.MainPage.Height / 7), App.Current.MainPage.Width - 20, (App.Current.MainPage.Height / 7) - 20), AbsoluteLayoutFlags.XProportional);
			listaNotifiche.Add(notifica);
			DestroyAfterTime(notifica);

			//Sposta tutte le notifiche più un basso
			for (int i = 0; i < listaNotifiche.Count; i++)
			{
				var item = listaNotifiche[i];
				item.TranslateTo(0, ((App.Current.MainPage.Height / 7) * (listaNotifiche.Count - i)));
			}
		}

		private async void DestroyAfterTime(Frame toDestroy)
		{
			await Task.Delay(5000);
			await toDestroy.FadeTo(0, 500);
			listaNotifiche.Remove(toDestroy);
			toDestroy.Parent = null;
		}
	}
}