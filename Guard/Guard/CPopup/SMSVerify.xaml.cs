using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Guard.CPopup
{
    public partial class SMSVerify : Popup
    {
        public SMSVerify()
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