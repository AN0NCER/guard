using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using Guard.Library;
using Newtonsoft.Json;
using SteamAuth;
using System.Collections.Generic;
using System.IO;


namespace Guard.Droid
{
    [BroadcastReceiver]
    public class WidgetService : BroadcastReceiver
    {
        private static SteamGuardAccount steamGuard;

        public override void OnReceive(Context context, Intent intent)
        {
            // Build the widget update for today
            RemoteViews updateViews = buildUpdate(context);

            // Push update for this widget to the home screen
            ComponentName thisWidget = new ComponentName(context, Java.Lang.Class.FromType(typeof(GuardWidgetClass)).Name);//"guard.GuardWidgetClass"
            AppWidgetManager manager = AppWidgetManager.GetInstance(context);
            manager.UpdateAppWidget(thisWidget, updateViews);
            Toast.MakeText(context, "UpDate", ToastLength.Short).Show();//Test
            Restart(context);
        }

        void Restart(Context context)
        {
            Intent intent = new Intent(context, typeof(WidgetService));
            PendingIntent pending = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Mutable);
            AlarmManager alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarm.Cancel(pending);
            alarm.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 30000, pending);

            
        }
        public RemoteViews buildUpdate(Context context)
        {
            if (steamGuard == null)
            {
                List<string> files = IO.Files;
                steamGuard = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(files[0]));
            }
            string code = steamGuard.GenerateSteamGuardCode();
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.guardwidget);
            views.SetTextViewText(Resource.Id.Code, code);
            return views;
        }

    }
}