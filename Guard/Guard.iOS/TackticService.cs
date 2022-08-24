using Guard.Interface;
using Guard.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(TackticService))]
namespace Guard.iOS
{
    public class TackticService : ITacktile
    {
        public void Tacktile()
        {
            // Initialize feedback
            var impact = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
            impact.Prepare();

            // Trigger feedback
            impact.ImpactOccurred();
        }
    }
}

