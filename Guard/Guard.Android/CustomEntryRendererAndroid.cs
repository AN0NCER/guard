using Guard;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using RefreshViewDemo.Droid;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRendererAndroid))]
namespace RefreshViewDemo.Droid
{
    public class CustomEntryRendererAndroid : EntryRenderer
    {
        public CustomEntryRendererAndroid(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            //if (Control != null)
            //{
            //    GradientDrawable gd = new GradientDrawable();
            //    gd.SetColor(Android.Graphics.Color.Transparent);
            //    this.Control.SetBackground(gd);
            //    this.Control.SetPadding(20, 0, 0, 0);

            //    this.Control.SetTextCursorDrawable(2131165279);//cursor.xml
                                                               
            //    CustomEntry customEntry = (CustomEntry)e.NewElement;

            //    if (customEntry.IsPasswordFlag)
            //        this.Control.InputType = InputTypes.TextVariationVisiblePassword;
            //}
        }

    }
}