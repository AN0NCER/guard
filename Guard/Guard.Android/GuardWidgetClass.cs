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
    [BroadcastReceiver(Label = "GuardWidget", Exported = true)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/guardwidgetprovider")]
    public class GuardWidgetClass : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {

        }
        public override void OnEnabled(Context context)
        {
            Intent intent = new Intent(context, typeof(WidgetService));
            PendingIntent pending = PendingIntent.GetService(context, 0, intent, PendingIntentFlags.Mutable);
            AlarmManager alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarm.Cancel(pending);
            alarm.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 1000, pending);
        }

    }
}