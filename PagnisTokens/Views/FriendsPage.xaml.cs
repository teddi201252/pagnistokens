using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace PagnisTokens.Views
{
    public partial class FriendsPage : ContentPage
    {
        private ICommand entryUnfocused;
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
            Console.WriteLine("Cerca!");
            //Metti la logica della ricerca qua
        }

    }
}
