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
    [BroadcastReceiver(Label = "GuardWidget", Exported = true)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    
    [IntentFilter(new string[] { "com.Guard.guard.ACTION_APPWIDGET_DELETED" })]
    [IntentFilter(new [] { "com.Guard.guard.ACTION_APPWIDGET_DISABLED" })]
    [IntentFilter(new [] { "com.Guard.guard.ACTION_APPWIDGET_ENABLED" })]

    [IntentFilter(new string[] { "com.Guard.guard.ACTION_CLOCK_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/guardwidgetprovider")]
    public class GuardWidgetClass : AppWidgetProvider
    {
        private static SteamGuardAccount steamGuard;
        private bool isDisabled = false;
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            //base.OnUpdate(context, appWidgetManager, appWidgetIds);
            Toast.MakeText(context, "UpDate", ToastLength.Short).Show();//Test


            if (steamGuard == null)
            {
                List<string> files = IO.Files;
                steamGuard = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(files[0]));
            }
            string code = steamGuard.GenerateSteamGuardCode();
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.guardwidget);
            views.SetTextViewText(Resource.Id.Code, code);
            appWidgetManager.UpdateAppWidget(appWidgetIds, views);
        }
        public override void OnReceive(Context context, Intent intent)
        {
            
            Toast.MakeText(context, "Restart", ToastLength.Short).Show();//Test

            ComponentName appWidgets = new ComponentName(context.PackageName, Java.Lang.Class.FromType(typeof(GuardWidgetClass)).Name);
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            int[] ids = appWidgetManager.GetAppWidgetIds(appWidgets);
            
            if (ids.Length > 0)
                OnUpdate(context, appWidgetManager, ids);

            Restart(context, new Intent(context, typeof(GuardWidgetClass)));
        }
        public override void OnEnabled(Context context)
        {
            //Intent intent = new Intent(context, typeof(WidgetService));
            //PendingIntent pending = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Mutable);
            //AlarmManager alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);
            //alarm.Cancel(pending);
            //alarm.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 30000, pending);

        }
        public override void OnDisabled(Context context)
        {
            isDisabled = true;
        }
        public override void OnRestored(Context context, int[] oldWidgetIds, int[] newWidgetIds)
        {
            isDisabled = true;
        }
        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            isDisabled = true;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        private void Restart(Context context, Intent intent)
        {
            if (isDisabled)
                return;

            PendingIntent pending = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Mutable);
            AlarmManager alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarm.Cancel(pending);
            alarm.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 30000, pending);
        }

    }
}