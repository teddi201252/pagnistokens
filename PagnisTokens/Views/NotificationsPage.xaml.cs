using System;
using System.Collections.Generic;
using System.Linq;
using FormsControls.Base;
using PagnisTokens.Fonts;
using PagnisTokens.Models;
using PagnisTokens.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace PagnisTokens.Views
{
    public partial class NotificationsPage : ContentPage, IAnimationPage
    {
        public IPageAnimation PageAnimation { get; } = new SlidePageAnimation { Duration = AnimationDuration.Short, Subtype = AnimationSubtype.FromRight };

        private NotificationSystem notificationSystem;

        public NotificationsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            notificationSystem = new NotificationSystem();
            AbsoluteRoot.Children.Add(notificationSystem, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
        }

        public void OnAnimationFinished(bool isPopAnimation) {
            if (isPopAnimation)
            {
                Navigation.RemovePage(this);
            }
        }

        public void OnAnimationStarted(bool isPopAnimation) {
            if (!isPopAnimation)
            {
                List<NotificationModel> listaNotifiche = DatabaseHelper.getAllNotificationsIdForCurrentUser();

                foreach (NotificationModel notifica in listaNotifiche)
                {
                    AbsoluteLayout absolute = new AbsoluteLayout {HeightRequest = 100 };
                    FlexOnIce.Children.Add(absolute);
                    StackLayout stackLabels = new StackLayout
                    {
                        Margin = 10,
                        Padding = 0,
                        Spacing = 5,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    absolute.Children.Add(stackLabels, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
                    Label titoloLabel = new Label
                    {
                        Text = notifica.title,
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold
                    };
                    stackLabels.Children.Add(titoloLabel);
                    Label msgLabel = new Label
                    {
                        Text = notifica.message,
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 16
                    };
                    stackLabels.Children.Add(msgLabel);
                    Button deleteButton = new Button
                    {
                        Padding = 0,
                        Text = FontLoader.CrossIcon,
                        FontFamily = "icon_font",
                        TextColor = Color.Black,
                        FontSize = 25,
                        BackgroundColor = Color.Transparent,
                        ClassId = notifica.id.ToString()
                    };
                    deleteButton.Clicked += async (sender, e) => //Cancella la notifica
                    {
                        await absolute.TranslateTo(App.Current.MainPage.Width, absolute.TranslationY);
                        double heightChilds = absolute.Height;
                        FlexOnIce.Children.Where(o => o.Y > absolute.Y).ForEach(o => o.TranslateTo(o.TranslationX, o.TranslationY - heightChilds));

                        DatabaseHelper.removeNotificationById(int.Parse(deleteButton.ClassId));
                    };
                    absolute.Children.Add(deleteButton, new Rectangle(1, 0, 40, 40), AbsoluteLayoutFlags.PositionProportional);
                    absolute.Children.Add(new BoxView
                    {
                        Color = Color.Gray
                    }, new Rectangle(0, 1, 1, 1), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
                }
            }
            
        }

        void CloseNotifications(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
