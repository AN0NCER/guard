using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Guard.CData;
using SteamAuth;
using Xamarin.Forms;

namespace Guard
{
    public partial class TradeView : ContentView
    {
        SteamGuardAccount _guardAccount; // Guard Account


        public ObservableCollection<User> Confirmations { get; set; } = new ObservableCollection<User>();

        public TradeView(SteamGuardAccount uGuard)
        {
            InitializeComponent();
            this.BindingContext = this;
            _guardAccount = uGuard;
            Confirmations.Add(new User { Name = "Anoncer", Image = "https://avatars.akamai.steamstatic.com/6bfb74e338765cf00d13d4b27473693c5e3c2ccf_full.jpg" });
            Confirmations.Add(new User { Name = "Anoncer", Image = "https://avatars.akamai.steamstatic.com/6bfb74e338765cf00d13d4b27473693c5e3c2ccf_full.jpg" });
            Confirmations.Add(new User { Name = "Anoncer", Image = "https://avatars.akamai.steamstatic.com/6bfb74e338765cf00d13d4b27473693c5e3c2ccf_full.jpg" });

            //_guardAccount.RefreshSession();
            //var trades = _guardAccount.FetchConfirmations();
            //var key = _guardAccount.GetConfirmationTradeOfferID(trades[0]);
        }

        void ClickGestureRecognizer_Clicked(System.Object sender, System.EventArgs e)
        {
        }
    }


    public class User
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }
}

