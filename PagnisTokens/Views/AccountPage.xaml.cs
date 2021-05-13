using System;
using System.Collections.Generic;
using System.IO;
using Devcorner.NIdenticon;
using Xamarin.Forms;
using Jdenticon;

namespace PagnisTokens.Views
{
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LabelUsername.Text = Application.Current.Properties["username"].ToString();
            Stream stream = new MemoryStream();
            stream = Identicon.FromValue(Application.Current.Properties["username"].ToString(), 150).SaveAsPng();
            
            ImageSource avatarSource = ImageSource.FromStream(() => stream);
            AvatarImage.Source = avatarSource;
        }
    }
}
