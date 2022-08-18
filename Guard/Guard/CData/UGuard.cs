using System;
using SteamAuth;
using System.ComponentModel;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms;

namespace Guard.CData
{
    public class UGuard : SteamGuardAccount, INotifyPropertyChanged
    {
        private string _secretCode = "00000";

        private double _progressTime = 0f;

        public PancakeView ItemView { get; set; } =new PancakeView() {
            HeightRequest = 7,
            WidthRequest = 7,
            CornerRadius = 4,
            BackgroundColor = Color.FromHex("#595E6E")
        };

        public string SecretCode
        {
            get => _secretCode;
            set
            {
                _secretCode = value;
                OnPropertyChanged(nameof(SecretCode));
            }
        }

        public double ProgressTime
        {
            get => _progressTime;
            set
            {
                _progressTime = value;
                OnPropertyChanged(nameof(ProgressTime));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

