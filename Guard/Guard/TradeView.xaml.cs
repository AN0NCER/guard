using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Guard.CData;
using SteamAuth;
using Xamarin.Forms;

namespace Guard
{
    public partial class TradeView : ContentView
    {
        SteamGuardAccount _guardAccount { get; set; } // Guard Account
        IEconService _econService { get; set; } //Trade Functions
        ISteamUser _steamUser { get; set; } //Steam User Information


        public ObservableCollection<UTrade> Confirmations { get; set; } = new ObservableCollection<UTrade>();

        public TradeView(SteamGuardAccount uGuard)
        {
            InitializeComponent();
            this.BindingContext = this;
            _guardAccount = uGuard;
            _econService = new IEconService(_guardAccount.Session);
            _steamUser = new ISteamUser(_guardAccount.Session);
            RefreshTrade.IsRefreshing = true;
        }

        void ClickGestureRecognizer_Clicked(System.Object sender, System.EventArgs e)
        {
        }

        void RefreshView_Refreshing(System.Object sender, System.EventArgs e)
        {
            new Thread(RefreshConfirmations).Start();
        }

        void AcceptTrade_Invoked(System.Object sender, System.EventArgs e)
        {
            SwipeItem item = sender as SwipeItem;
            UTrade trades = item.BindingContext as UTrade;
            bool acceptConfirmation = _guardAccount.AcceptConfirmation(trades.Confirmation);
            if (acceptConfirmation)
                Confirmations.Remove(trades);
        }

        void DeclineTrade_Invoked(System.Object sender, System.EventArgs e)
        {
            SwipeItem item = sender as SwipeItem;
            UTrade trades = item.BindingContext as UTrade;
            bool denyConfirmation = _guardAccount.DenyConfirmation(trades.Confirmation);
            if (denyConfirmation)
                Confirmations.Remove(trades);
        }

        void RefreshConfirmations()
        {
            Confirmations.Clear();
            bool refreshing = _guardAccount.RefreshSession();
            if (!refreshing)
            {
                Dispatcher.BeginInvokeOnMainThread(() => RefreshTrade.IsRefreshing = false);
                return;
            }
            try
            {
                Confirmation[] trades = _guardAccount.FetchConfirmations();
                for (int i = 0; i < trades.Length; i++)
                {
                    TradeResponseOffer trade = _econService.GetTradeOffer(trades[i].Creator);
                    SteamResponseUsers users = _steamUser.GetPlayerSummaries(_guardAccount.Session.SteamID.ToString(),
                        Util.ConvertSteam64(trade.TradeResponse.Offer.AccountidOther).ToString());
                    Confirmations.Add(new UTrade
                    {
                        Response = trade.TradeResponse,
                        Confirmation = trades[i],
                        AccountNames = new AccountName
                        {
                            NameAccount = users.UserResponse.Players[0],
                            NameOther = users.UserResponse.Players[1]
                        }
                    });
                }
            }
            catch
            {
                Dispatcher.BeginInvokeOnMainThread(() => RefreshTrade.IsRefreshing = false);
                return;
            }
            Dispatcher.BeginInvokeOnMainThread(() => RefreshTrade.IsRefreshing = false);
        }
    }
}