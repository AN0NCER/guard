using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Guard.CPopup
{
    public partial class MailVerify : Popup
    {
        public MailVerify()
        {
            InitializeComponent();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (Code.Text == null)
                return;
            Dismiss(Code.Text);
        }
    }
}

