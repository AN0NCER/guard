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
using System.Xml.Linq;
using System.Collections.Specialized;

namespace Guard
{
    public partial class MainPage : ContentPage
    {
        Thread TGUARD; // Thread update Guard Code

        SteamGuardAccount _guardAccount; // Guard Account 

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

            IO.Files.ForEach(x => {
                Guards.Add(JsonConvert.DeserializeObject<UGuard>(File.ReadAllText(x)));
            });
        }

        //Getting and updating the passcode
        void GuardSecretCode()
        {
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

        //Copy Guard Code
        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
        }
    }
}
