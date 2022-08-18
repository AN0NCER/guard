using System;
using System.Collections.Generic;
using Guard.CData;
using SteamAuth;
using Xamarin.Forms;

namespace Guard
{
    public partial class TradeView : ContentView
    {
        SteamGuardAccount _guardAccount; // Guard Account 

        public TradeView(SteamGuardAccount uGuard)
        {
            InitializeComponent();
            _guardAccount = uGuard;

            var trades = _guardAccount.FetchConfirmations();
            var key = _guardAccount.GetConfirmationTradeOfferID(trades[0]);
        }
    }
}

