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
        }

        private void CercaPersone(object obj)
        {
            ListaAmici.Children.Clear();
            //Metti la logica della ricerca qua
            List<UserModel> usersFound = DatabaseHelper.searchUsersByUsername(EntrySearch.Text);

			foreach (UserModel user in usersFound)
			{
				if (user.id != int.Parse(Application.Current.Properties["id"].ToString()))
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
                            Text = FontLoader.CrossIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Red,
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
                            ClassId = user.id.ToString()
                        };
                        addFriendBtn.Clicked += (sender, e) => {
                            Console.WriteLine("Aggiungi " + addFriendBtn.ClassId);
                        };
                        stackUser.Children.Add(addFriendBtn);
                    }
                }

            }
        }

		public void OnAnimationStarted(bool isPopAnimation)
		{
			if (!isPopAnimation)
			{
                friendList = DatabaseHelper.getAllFriendsOfCurrentUser();

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
                        stackUser.Children.Add(new Button
                        {
                            Text = FontLoader.CrossIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Red,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand
                        });
                        stackUser.Children.Add(new Button
                        {
                            Text = FontLoader.PlusIcon,
                            FontFamily = "icon_font",
                            Padding = 0,
                            FontSize = 25,
                            TextColor = Color.Green,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand
                        });
                    }
                    else
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

                }
            }
		}

        public void OnAnimationFinished(bool isPopAnimation) { }
	}
}
