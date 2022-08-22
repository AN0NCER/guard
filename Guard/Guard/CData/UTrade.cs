using System;
using SteamAuth;
using static SteamAuth.SteamResponseUsers;

namespace Guard.CData
{
    public class UTrade
    {
        public Confirmation Confirmation { get; set; }
        public AccountName AccountNames { get; set; }
        public TradeResponseOffer.Response Response { get; set; }
    }

    public class AccountName
    {
        public Player NameAccount { get; set; }
        public Player NameOther { get; set; }
    }
}