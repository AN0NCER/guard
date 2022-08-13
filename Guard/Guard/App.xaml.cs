using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Guard
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Check isset guard file
            string document = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/acc/";
            if (!Directory.Exists(document))
                Directory.CreateDirectory(document);

            string[] files = Directory.GetFiles(document, "*.guard");
            if (files.Length <= 0)
                Application.Current.MainPage = new FirstLogin();
            else
                Application.Current.MainPage = new MainPage();
        }

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
