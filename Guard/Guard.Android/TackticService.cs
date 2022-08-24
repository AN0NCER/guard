using Guard.Droid;
using Guard.Interface;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(TackticService))]
namespace Guard.Droid
{
    public class TackticService : ITacktile
    {
        public void Tacktile() => Vibration.Vibrate(250);
    }
}