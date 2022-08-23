using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;


namespace Guard.Droid
{
    [BroadcastReceiver(Label = "GuardWidget", Exported = true)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [IntentFilter(new string[] { "com.Guard.guard.ACTION_CLOCK_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/guardwidgetprovider")]
    public class GuardWidgetClass : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);
        }
        public override void OnEnabled(Context context)
        {
            Intent intent = new Intent(context, typeof(WidgetService));
            PendingIntent pending = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Mutable);
            AlarmManager alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarm.Cancel(pending);
            alarm.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 30000, pending);

        }
    }
}