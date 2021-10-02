using FormsControls.Base;
using Jdenticon;
using PagnisTokens.Fonts;
using PagnisTokens.Models;
using PagnisTokens.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace PagnisTokens.Views
{
    public partial class FriendsPage : ContentPage, IAnimationPage
    {
        public IPageAnimation PageAnimation { get; } = new SlidePageAnimation { Duration = AnimationDuration.Short, Subtype = AnimationSubtype.FromBottom };
        private ICommand entryUnfocused;
        private NotificationSystem notificationSystem;
        List<UserModel> friendList = null;

        public ICommand EntryUnfocused
        {
            get { return entryUnfocused; }
            protected set
            {
                entryUnfocused = value;
                OnPropertyChanged("EntryUnfocused");
            }

        }

		public FriendsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
            EntryUnfocused = new Command(CercaPersone);
            notificationSystem = new NotificationSystem();
            AbsoluteRoot.Children.Add(notificationSystem, new Xamarin.Forms.Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
        }

        private async void CercaPersone(object obj)
        {
            ListaAmici.Children.Clear();
            //Metti la logica della ricerca qua
            List<UserModel> usersFound = await App.apiHelper.searchUsersByUsername(EntrySearch.Text);

			foreach (UserModel user in usersFound)
			{
				if (user.id != Application.Current.Properties["id"].ToString())
				{
                    StackLayout stackUser = new StackLayout {
                        Margin = new Thickness(10,0,10,0),
                        Orientation = StackOrientation.Horizontal,
                        Padding = 0,
                        HeightRequest = 50
                    };
                    ListaAmici.Children.Add(stackUser);
                    stackUser.Children.Add(new Image
                    {
                        Source = ImageSource.FromStream(() => Identicon.FromValue(user.username, 150).SaveAsPng()),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.Start,
                        HeightRequest = 50,
                        WidthRequest = 50,
                        Clip = new EllipseGeometry(new Point(25, 25), 25, 25)
                    });
                    stackUser.Children.Add(new Label {
                        Text = user.username,
                        TextColor = Color.Black,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    });
					if (friendList.Where(o => o.id == user.id).ToList().Count > 0)
					{
                        stackUser.Children.Add(new Button
                        {
                            Text = FontLoader.FriendIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Black,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand
                        });
                    }
					else
					{
                        Button addFriendBtn = new Button
                        {
                            Text = FontLoader.PlusIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Green,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                        };
                        addFriendBtn.Clicked += async (sender, e) => {
                            await App.apiHelper.createNewFriendship(user.id);
                            App.apiHelper.sendNotification(user.id, "Nuova richiesta", App.Current.Properties["username"] + " ti ha inviato una richiesta di amicizia");
                            notificationSystem.AddNewNotification("Aggiunto!","Hai mandato una richiesta di amicizia a " + user.username, Color.Green);
                        };
                        stackUser.Children.Add(addFriendBtn);
                    }
                }

            }
        }

		public async void OnAnimationStarted(bool isPopAnimation)
		{
			if (!isPopAnimation)
			{
                friendList = await App.apiHelper.getAllFriendsOfCurrentUser();

                foreach (UserModel user in friendList)
                {
                    StackLayout stackUser = new StackLayout
                    {
                        Margin = new Thickness(10, 0, 10, 0),
                        Orientation = StackOrientation.Horizontal,
                        Padding = 0,
                        HeightRequest = 50
                    };
                    ListaAmici.Children.Add(stackUser);
                    stackUser.Children.Add(new Image
                    {
                        Source = ImageSource.FromStream(() => Identicon.FromValue(user.username, 150).SaveAsPng()),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.Start,
                        HeightRequest = 50,
                        WidthRequest = 50,
                        Clip = new EllipseGeometry(new Point(25, 25), 25, 25)
                    });
                    stackUser.Children.Add(new Label
                    {
                        Text = user.username,
                        TextColor = Color.Black,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    });

                    if (user.friendStatusWithCurrent == "toAccept")
                    {
                        Button refuseBtn = new Button
                        {
                            Text = FontLoader.CrossIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Red,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand
                        };
                        refuseBtn.Clicked += async (sender, e) => {
                            await App.apiHelper.refuseFriendshipByIds(user.id, App.Current.Properties["id"].ToString());
                            App.apiHelper.sendNotification(user.id, "Peggio della friendzone", App.Current.Properties["username"] + " ha rifiutato la tua richiesta d'amicizia :(");
                            notificationSystem.AddNewNotification("Rifiutato", "Hai rifiutato la richiesta di " + user.username, Color.Red);
                            ListaAmici.Children.Remove(stackUser);
                            friendList.Remove(friendList.Where(o=>o.id == user.id).ToList()[0]);
                        };
                        stackUser.Children.Add(refuseBtn);

                        Button acceptBtn = new Button
                        {
                            Text = FontLoader.PlusIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Green,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand
                        };
                        acceptBtn.Clicked += async (sender, e) => {
                            await App.apiHelper.acceptFriendshipByIds(user.id, App.Current.Properties["id"].ToString());
                            App.apiHelper.sendNotification(user.id, "Accettato!", App.Current.Properties["username"] + " ha accettato la tua richiesta d'amicizia :)");
                            notificationSystem.AddNewNotification("Accettato", "Hai accettato la richiesta di " + user.username, Color.Green);
                            friendList.Where(o => o.id == user.id).ToList()[0].friendStatusWithCurrent = "accepted";

                            stackUser.Children.Remove(refuseBtn);
                            stackUser.Children.Remove(acceptBtn);
                            stackUser.Children.Add(new Button
                            {
                                Text = FontLoader.FriendIcon,
                                FontFamily = "icon_font",
                                Padding = 0,
                                FontSize = 25,
                                TextColor = Color.Black,
                                BackgroundColor = Color.Transparent,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                HorizontalOptions = LayoutOptions.EndAndExpand
                            });
                        };
                        stackUser.Children.Add(acceptBtn);
                    }
                    else
                    {
                        stackUser.Children.Add(new Label
                        {
                            Text = await App.apiHelper.getBalanceFromWallet(user.walletid),
                            TextColor = Color.Black,
                            FontSize = 18,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center
                        });
                        stackUser.Children.Add(new Button
                        {
                            Text = FontLoader.FriendIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Black,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand
                        });
                    }

                }
            }
		}

        public void OnAnimationFinished(bool isPopAnimation) { }
	}
}
