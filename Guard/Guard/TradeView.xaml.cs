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
        }

        void ClickGestureRecognizer_Clicked(System.Object sender, System.EventArgs e)
        {
        }

        async void RefreshView_Refreshing(System.Object sender, System.EventArgs e)
        {
            new Thread(() =>
            {
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
                        Confirmations.Add(new UTrade {
                            Response = trade.TradeResponse,
                            IDTrade = trades[i].Creator,
                            AccountNames = new AccountName {
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

            }).Start();
        }

        void AcceptTrade_Invoked(System.Object sender, System.EventArgs e)
        {
        }

        void DeclineTrade_Invoked(System.Object sender, System.EventArgs e)
        {
        }
    }
}