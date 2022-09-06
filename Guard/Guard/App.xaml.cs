using System;
using System.IO;
using Guard.Library;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guard
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Page page = new MainPage();

            if (TestIO.Files.Count <= 0)
                page = new FirstLogin();

            ToPage(page);
        }

        private void ToPage(Page page) => Application.Current.MainPage = page;

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
