using System;
using System.Collections.Generic;
using ZXing.QrCode;

using Xamarin.Forms;
using System.IO;

using ZXing.Mobile;
using ZXing.QrCode.Internal;
using Guard.CPopup;
using Xamarin.CommunityToolkit.Extensions;

namespace Guard
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        void ShowQRCODE()
        {
            Navigation.ShowPopupAsync(new QRExport());
        }

        void btnExport_Clicked(System.Object sender, System.EventArgs e)
        {
            ShowQRCODE();
        }
    }
}
