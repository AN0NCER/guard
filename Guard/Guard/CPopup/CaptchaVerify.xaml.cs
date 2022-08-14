using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Guard.CPopup
{
    public partial class CaptchaVerify : Popup
    {
        public CaptchaVerify(string urlImage)
        {
            InitializeComponent();
            Captcha.Source = new UriImageSource { Uri = new Uri(urlImage) };
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (Code.Text == null)
                return;
            Dismiss(Code.Text);
        }
    }
}

