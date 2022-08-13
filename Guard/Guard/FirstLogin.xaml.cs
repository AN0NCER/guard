using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SteamAuth;
using Xamarin.CommunityToolkit.Extensions;
using System.Threading.Tasks;
using Guard.CPopup;
using Newtonsoft.Json;
using System.IO;
using Xamarin.CommunityToolkit.UI.Views;

namespace Guard
{
    public partial class FirstLogin : ContentPage
    {
        private bool _isDialog { get; set; }


        public FirstLogin(bool IsDialog = false)
        {
            _isDialog = IsDialog;
            InitializeComponent();
        }

        async void Login_Clicked(System.Object sender, System.EventArgs e)
        {


            NextPage();


            //Check on empty Entry
            if (Username.Text == null && Password.Text == null)
                return;

            Login.IsEnabled = false;

            UserLogin login = new UserLogin(Username.Text, Password.Text);

            LoginResult response = LoginResult.BadCredentials;
            while ((response = login.DoLogin()) != LoginResult.LoginOkay)
            {
                switch (response)
                {
                    case LoginResult.NeedEmail:
                        string code = (string)await Navigation.ShowPopupAsync(new MailVerify());
                        login.EmailCode = code;
                        break;
                    case LoginResult.NeedCaptcha:
                        string captcha = (string)await Navigation.ShowPopupAsync(new CaptchaVerify(APIEndpoints.COMMUNITY_BASE + "/public/captcha.php?gid=" + login.CaptchaGID));
                        login.CaptchaText = captcha;
                        break;
                    case LoginResult.Need2FA:
                        await DisplayAlert("Need2FA", "Steam Guard is already enabled on this account. Import or disable Guard in your Steam account.", "OK");
                        Username.Text = null;
                        Password.Text = null;
                        Login.IsEnabled = true;
                        return;
                    case LoginResult.BadRSA:
                        await DisplayAlert("BadRSA", "Disable all Device Guards in Steam to resolve this issue.", "OK");
                        Login.IsEnabled = true;
                        break;
                    case LoginResult.GeneralFailure:
                        await DisplayAlert("GeneralFailure", "Please restart app.", "OK");
                        break;
                    case LoginResult.TooManyFailedLogins:
                        await DisplayAlert("TooManyFailedLogins", "Too many login attempts. Please try again later.", "OK");
                        break;
                }
            }

            AuthenticatorLinker linker = new AuthenticatorLinker(login.Session);

            var result = linker.AddAuthenticator();
            if (result != AuthenticatorLinker.LinkResult.AwaitingFinalization)
            {
                Login.IsEnabled = true;
                //Need to add telephone to steam )
                return;
            }
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/acc/";
                if (!Directory.Exists(documents))
                    Directory.CreateDirectory(documents);
                var file = Path.Combine(documents, linker.LinkedAccount.AccountName + ".guard");
                string sgFile = JsonConvert.SerializeObject(linker.LinkedAccount, Formatting.Indented);
                File.WriteAllText(file, sgFile);
            }
            catch (Exception ex)
            {
                Login.IsEnabled = true;
                return;
            }

            string smsCode = (string)await Navigation.ShowPopupAsync(new SMSVerify());
            var linkResult = linker.FinalizeAddAuthenticator(smsCode);
            if (linkResult != AuthenticatorLinker.FinalizeResult.Success)
            {
                Console.WriteLine("Unable to finalize authenticator: " + linkResult);
                Login.IsEnabled = true;
                return;
            }

            NextPage();
        }

        private async void NextPage()
        {
            if (_isDialog)
                await Application.Current.MainPage.Navigation.PopModalAsync();
            else
            {
                var navigationPage = new NavigationPage(new MainPage());
                if (Device.RuntimePlatform == Device.iOS)
                {
                    Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetModalPresentationStyle(
                        navigationPage.On<Xamarin.Forms.PlatformConfiguration.iOS>(),
                        Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FullScreen);
                }
                await Navigation.PushModalAsync(navigationPage);
            }
        }
    }
}

