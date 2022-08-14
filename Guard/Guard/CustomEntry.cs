using System;
using Xamarin.Forms;

namespace Guard
{
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty IsPasswordFlagProperty =
        BindableProperty.Create("IsPasswordFlag", typeof(bool), typeof(CustomEntry), defaultBindingMode: BindingMode.OneWay);
        public bool IsPasswordFlag
        {
            get { return (bool)GetValue(IsPasswordFlagProperty); }
            set { SetValue(IsPasswordFlagProperty, value); }
        }
    }
}