using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Guard.Library;
using Xamarin.CommunityToolkit;
using Guard.CData;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using SteamAuth;
using System.Collections.Specialized;
using Xamarin.Forms.Internals;
using Xamarin.Essentials;
using System.Windows.Input;
using Guard.Interface;
using System.Runtime.CompilerServices;

namespace Guard
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged, IAccountMove
    {
        Thread TGUARD; // Thread update Guard Code

        SteamGuardAccount _guardAccount; // Guard Account 
        bool isTradeActive = false;
        TradeView grid;

        public ObservableCollection<UGuard> Guards { get; set; } = new ObservableCollection<UGuard>(); //List Guards Accounts
        public UGuard CurGuard { get; set; } //User Guard Current Selected

        public MainPage()
        {
            InitializeComponent();
            Guards.CollectionChanged += Guards_CollectionChanged;
            ParseGuardFiles();
            BindingContext = this;
            GuardCarouse.ScrollTo(0, position: ScrollToPosition.MakeVisible);
        }

        //If changed Collection Steam Guard
        void Guards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Loop if more than one element
            if (Guards.Count < 1)
                GuardCarouse.Loop = true;
            else
                GuardCarouse.Loop = false;
        }

        /// <summary>
        /// Parsing Guard Files
        /// </summary>
        void ParseGuardFiles()
        {
            if (IO.Files.Count <= 0)
                return; //Exit Program. Somewhere it looks like a bug in the code;

            IO.Files.ForEach(x =>
            {
                //Adding guards list account
                Guards.Add(JsonConvert.DeserializeObject<UGuard>(File.ReadAllText(IO.PathGuardFile + x.Path)));
            });

            new Thread(AddItemViews).Start();
        }
        //Getting and updating the passcode
        void GuardSecretCode()
        {
            new Thread(SetItemColor).Start();
            while (true)
            {
                string s = _guardAccount.GenerateSteamGuardCode();
                CurGuard.SecretCode = s;
                for (double i = 0f; i < 1.0f; i += 0.1)
                {
                    CurGuard.ProgressTime = i;
                    Thread.Sleep(3000);
                }
            }
        }

        //Change Guard Code another account
        void GuardCarouse_CurrentItemChanged(System.Object sender, Xamarin.Forms.CurrentItemChangedEventArgs e)
        {
            //Stop thread if started
            if (TGUARD != null)
                TGUARD.Abort();

            string content = File.ReadAllText(IO.GetFileByName(CurGuard.AccountName));
            _guardAccount = JsonConvert.DeserializeObject<SteamGuardAccount>(content);

            TGUARD = new Thread(GuardSecretCode);
            TGUARD.Start();
        }

        //Remove Guard Auth
        async void RemAuth_Clicked(System.Object sender, System.EventArgs e)
        {
            bool question = await DisplayAlert("Delete?", $"Are you sure you want to remove the authenticator from your account ({CurGuard.AccountName})?", "Yes", "No");
            if (!question)
                return;

            bool refreshSession = _guardAccount.RefreshSession();
            bool answer = _guardAccount.DeactivateAuthenticator();

            if (!answer && refreshSession)
                return;

            answer = IO.Remove.ByName(CurGuard.AccountName);

            if (!answer)
                return;


            if ((Guards.Count - 1) <= 0)
            {
                TGUARD.Abort();
                Application.Current.MainPage = new FirstLogin();
            }
            else
            {
                ItemViewer.Children.Remove(CurGuard.ItemView);
                Guards.Remove(CurGuard);
            }
        }

        //Add point accounts
        void AddItemViews()
        {
            Guards.ForEach(x =>
            {
                ItemViewer.Children.Add(x.ItemView);
            });
        }

        //Change Color d't active Item
        void SetItemColor()
        {
            Guards.ForEach(x =>
            {
                if (x == CurGuard)
                    Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        CurGuard.ItemView.BackgroundColor = Color.FromHex("#31BCEC");
                    });
                else
                    Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                        x.ItemView.BackgroundColor = Color.FromHex("#595E6E");
                    });
            });
        }

        //Share Secret File
        async void ShareFile_Clicked(System.Object sender, System.EventArgs e)
        {
            string filePath = IO.GetFileByName(_guardAccount.AccountName);

            if (!File.Exists(filePath))
                return;

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = $"Secret Guard {_guardAccount.AccountName}",
                File = new ShareFile(filePath)
            });
        }
        
        private void NavigationPage_Disappearing(object sender, EventArgs e)
        {
            new Thread(UpdateListAccounts).Start();
        }

        /// <summary>
        /// Update Accounts From File and List
        /// </summary>
        void UpdateListAccounts()
        {
            IO.Update();
            if (IO.Files.Count > Guards.Count)
            {
                UGuard addGuard = null;

                IO.Files.ForEach(x =>
                {
                    UGuard tmpGuard = JsonConvert.DeserializeObject<UGuard>(File.ReadAllText(IO.GetFileByName(x.Name)));

                    Guards.ForEach(x =>
                    {
                        if (x.AccountName != tmpGuard.AccountName)
                            addGuard = tmpGuard;
                    });
                });

                if (addGuard != null)
                {
                    Dispatcher.BeginInvokeOnMainThread(() => ItemViewer.Children.Add(addGuard.ItemView));
                    Guards.Add(addGuard);
                }

            }
        }

        bool IsAnimate = false;

        //Show Trade Control
        void TardeBtn_Clicked(System.Object sender, System.EventArgs e)
        {
            if (isTradeActive || IsAnimate)
                return;

            IsAnimate = true;
            grid = new TradeView(_guardAccount);
            ViewContent.Children.Add(grid);
            SwitchAnimate.Commit(this, "view", length: 250, easing: Easing.SinInOut, finished: (v, c) => IsAnimate = false);
            isTradeActive = true;

            SwitchButton(sender as Button, GuardBtn);
        }

        private void GuardBtn_Clicked(object sender, EventArgs e)
        {
            if (!isTradeActive || IsAnimate)
                return;

            IsAnimate = true;
            SwitchAnimate.Commit(this, "view", length: 250, easing: Easing.SinInOut, finished: (v, c) => { ViewContent.Children.Remove(grid); IsAnimate = false; });
            isTradeActive = false;

            SwitchButton(sender as Button, TardeBtn);
        }

        void SwitchButton(Button btn, Button active)
        {
            btn.BackgroundColor = active.BackgroundColor;
            btn.TextColor = active.TextColor;
            active.BackgroundColor = Color.Transparent;
            active.TextColor = Color.FromHex("#595D6E");
        }

        Animation SwitchAnimate
        {
            get
            {
                Animation animation = new Animation();
                if (isTradeActive)
                    animation = new Animation((v) => grid.Margin = new Thickness(v, 0, 0, 0), 0, grid.Width);
                else
                    animation = new Animation((v) => grid.Margin = new Thickness(v, 0, 0, 0), grid.Width, 0);

                return animation;
            }
        }

        //Copy Guard Code
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            DependencyService.Get<ITacktile>().Tacktile();
            await Clipboard.SetTextAsync(CurGuard.SecretCode);
        }

        async void SettingBtn_Clicked(System.Object sender, System.EventArgs e) => await LoadPageAsync(new SettingPage());
        //Adding new Account or Export
        async void AddAuth_Clicked(System.Object sender, System.EventArgs e) => await LoadPageAsync(new FirstLogin(true));

        async Task LoadPageAsync(ContentPage page)
        {
            var navigationPage = new NavigationPage(page);
            navigationPage.Disappearing += NavigationPage_Disappearing;
            if (Device.RuntimePlatform == Device.iOS)
            {
                Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetModalPresentationStyle(
                    navigationPage.On<Xamarin.Forms.PlatformConfiguration.iOS>(),
                    Xamarin.Forms.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.PageSheet);
            }
            await Navigation.PushModalAsync(navigationPage);
        }

        public void AccountMove(int a, int b)
        {
            Guards.Move(a, b);
            ItemViewer.Children.Clear();
            AddItemViews();
        }
    }
}