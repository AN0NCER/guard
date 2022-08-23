using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamAuth;
using Newtonsoft.Json;
using Guard.Library;
using System.IO;
using System.Threading;
using Java.Util;

namespace Guard.Droid
{
    [BroadcastReceiver(Label = "GuardWidget", Exported = false)]
    
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [IntentFilter(new string[] { "android.appwidget.action.TIME_CHANGED" })]
    [IntentFilter(new string[] { "android.appwidget.action.TIMEZONE_CHANGED" })]

    [IntentFilter(new string[] { "com.Guard.guard.ACTION_CLOCK_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/guardwidgetprovider")]
    public class GuardWidgetClass : AppWidgetProvider
    {
        private SteamGuardAccount steamGuard;
        public static String ACTION_CLOCK_UPDATE = "com.Guard.guard.ACTION_CLOCK_UPDATE";
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
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
        public override void OnEnabled(Context context)
        {
            //Intent intent = new Intent(context, typeof(WidgetService));
            //PendingIntent pending = PendingIntent.GetService(context, 0, intent, PendingIntentFlags.Mutable);
            //AlarmManager alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);
            //alarm.Cancel(pending);
            //alarm.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 1000, pending);


            startTicking(context);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            string action = intent.Action;

            if (ACTION_CLOCK_UPDATE == action || Intent.ActionTimeChanged.Equals(action) || Intent.ActionTimezoneChanged.Equals(action))
            {
                ComponentName appWidgets = new ComponentName(context, Java.Lang.Class.FromType(typeof(GuardWidgetClass)).Name);
                AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);

                int[] ids = appWidgetManager.GetAppWidgetIds(appWidgets);

                if (ids.Length > 0)
                {
                    OnUpdate(context, appWidgetManager, ids);
                }
            }
        }

        private void startTicking(Context context)
        {
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);


            //Calendar c = Calendar.GetInstance(Locale.Us);
            //c.setTimeInMillis(System.currentTimeMillis());
            //c.Set(CalendarField.Second, 0);
            //c.Set(CalendarField.Millisecond, 0);
            //c.Add(CalendarField.Minute, 1);


            alarmManager.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 3000, createUpdate(context));
        }

        private PendingIntent createUpdate(Context context)
        {
            return PendingIntent.GetBroadcast(context, 0, new Intent(ACTION_CLOCK_UPDATE), PendingIntentFlags.Immutable);
        }
    }
}
//https://github.com/xxv/24hAnalogWidget/blob/main/phoneWidget/src/main/AndroidManifest.xml