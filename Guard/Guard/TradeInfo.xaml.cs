using System;
using System.Collections.Generic;
using System.Threading;
using Guard.CData;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PancakeView;
using static SteamAuth.SteamResponseUsers;

namespace Guard
{
    public partial class TradeInfo : ContentPage
    {
        public string MarketName { get; set; } = "NULL";
        public long Appid { get; set; } = 753;
        public string Type { get; set; } = "NULL";
        public string DescriptionValue { get; set; } = "NULL";
        public string TradeOfferState { get; set; } = "NULL";
        public string IconUrl { get; set; } = "NULL";
        public string Avatar { get; set; } = "NULL";

        public Thread threadExpiration { get; set; }

        public TradeInfo(UTrade uTrade)
        {
            InitializeComponent();
            MarketName = uTrade.Response.Descriptions[0].MarketName;
            Appid = uTrade.Response.Descriptions[0].Appid;
            Type = uTrade.Response.Descriptions[0].Type;
            DescriptionValue = uTrade.Response.Descriptions[0].Descriptions[0].Value;
            Tags(uTrade.Response.Descriptions[0].Tags);
            TradeOfferState = GetStatus(uTrade.Response.Offer);
            IconUrl = uTrade.Response.Descriptions[0].IconUrl;
            Avatar = GetIconAccount(new Player[] {uTrade.AccountNames.NameAccount, uTrade.AccountNames.NameOther }, uTrade.Response.Offer.AccountidOther);
            threadExpiration = new Thread(() => { Expiration(uTrade.Response.Offer.ExpirationTime); });
            threadExpiration.Start();
            BindingContext = this;
        }

        public void Tags(List<SteamAuth.TradeResponseOffer.Tag> tags)
        {
            TagViews.Children.Clear();
            tags.ForEach(x =>
            {
                PancakeView pV = new PancakeView()
                {
                    Padding = new Thickness(10, 7),
                    BackgroundColor = Color.FromHex("#3F6CB0"),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(0, 0, 5, 0),
                    Content = new Label
                    {
                        Text = x.LocalizedTagName,
                        TextColor = Color.FromHex("#ffffff"),
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold
                    }
                };
                TagViews.Children.Add(pV);
            });
        }

        public string GetStatus(SteamAuth.TradeResponseOffer.Offer offer)
        {
            switch (offer.TradeOfferState)
            {
                case 1:
                    return "Invalid";
                case 2:
                    return "Active";
                case 3:
                    return "Accepted";
                case 4:
                    return "Countered";
                case 5:
                    return "Expired";
                case 6:
                    return "Canceled";
                case 7:
                    return "Declined";
                case 8:
                    return "Invalid Items";
                case 9:
                    return "Created Needs Confirmation";
                case 10:
                    return "Canceled By Second Factor";
                case 11:
                    return "In Esscrow";
                default:
                    return "";
            }
        }

        public string GetIconAccount(Player[] players, long accountid)
        {
            string url = "";
            players.ForEach(x =>
            {
                if(x.Steamid == SteamAuth.Util.ConvertSteam64(accountid).ToString())
                {
                    url = x.Avatar.AbsoluteUri;
                    return;
                }
            });
            return url;
        }

        public void Expiration(long exp)
        {
            DateTime time = UnixTimeStampToDateTime(exp);
            while ((time - DateTime.Now) != TimeSpan.Zero)
            {
                var t = DateTime.Now.Subtract(time);
                Dispatcher.BeginInvokeOnMainThread(() => ExpirationValue.Text = t.ToString(@"hh\:mm\:ss"));
                Thread.Sleep(1000);
            }
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}