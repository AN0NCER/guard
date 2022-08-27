using System;
using System.Collections.Generic;
using Guard.CData;
using Xamarin.Forms;

namespace Guard
{
    public partial class TradeInfo : ContentPage
    {
        public string MarketName { get; set; } = "NULL";
        public long Appid { get; set; } = 753;
        public string Type { get; set; } = "NULL";
        public string DescriptionValue { get; set; } = "NULL";

        public TradeInfo(UTrade uTrade)
        {
            InitializeComponent();
            MarketName = uTrade.Response.Descriptions[0].MarketName;
            Appid = uTrade.Response.Descriptions[0].Appid;
            Type = uTrade.Response.Descriptions[0].Type;
            DescriptionValue = uTrade.Response.Descriptions[0].Descriptions[0].Value;
            BindingContext = this;
        }
    }
}

