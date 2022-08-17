using System;
using System.IO;
using SteamAuth;
using Guard.CPopup;
using Xamarin.Forms;
using Guard.Library;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;

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
            //NextPage();
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

            var write = IO.AddAndWrite(linker.LinkedAccount.AccountName,
                JsonConvert.SerializeObject(linker.LinkedAccount, Formatting.Indented));

            if (!write)
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


        //Import Account
        async void IportAcc_Clicked(System.Object sender, System.EventArgs e)
        {
            var customFileType =
                new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "com.Guard.guard" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "guard", "json" } }
                });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Pick Guard Account",
                FileTypes = customFileType
            });

            if (pickResult == null)
                return;

            try
            {
                SteamGuardAccount steamGuard = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(pickResult.FullPath));
                if (String.IsNullOrEmpty(steamGuard.AccountName))
                    return;

                if (File.Exists(IO.GetFileByName(steamGuard.AccountName)))
                {
                    bool answer = await DisplayAlert("Replace?", $"An account with the same name ({steamGuard.AccountName}) already exists. Do you want to replace?", "Yes", "No");

                    if (!answer)
                        return;

                    IO.RemoveFileByName(steamGuard.AccountName);
                }

                IO.AddAndWrite(steamGuard.AccountName, JsonConvert.SerializeObject(steamGuard));
                NextPage();
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}

