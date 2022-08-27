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
    [BroadcastReceiver(Label = "GuardWidget", Icon = "@mipmap/ic_launcher_round", Exported = false)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [IntentFilter(new string[] { "com.Guard.guard.ACTION_CLOCK_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/guardwidgetprovider")]
    public class GuardWidgetClass : AppWidgetProvider
    {
        private static SteamGuardAccount steamGuard = null;
        private static string GuardUpDate = "com.Guard.OnUpdate";
        private static string CodeBtnTag = "com.Guard.CodeBtn";

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.guardwidget);

            views.SetTextViewText(Resource.Id.CodeBtn, GetCodeGuard() == null ? "*****" : GetCodeGuard());
            views.SetOnClickPendingIntent(Resource.Id.CodeBtn, createPendingIntent(context, CodeBtnTag));

            appWidgetManager.UpdateAppWidget(appWidgetIds, views);

        }
        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            if (steamGuard == null)
                return;

            if (intent.Action.Equals(GuardUpDate))
            {
                ComponentName appWidgets = new ComponentName(context.PackageName, Java.Lang.Class.FromType(typeof(GuardWidgetClass)).Name);
                AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
                int[] ids = appWidgetManager.GetAppWidgetIds(appWidgets);

                if (ids.Length > 0)
                    OnUpdate(context, appWidgetManager, ids);
            }
            if (intent.Action.Equals(CodeBtnTag))
            {
                ClipboardManager clipboardManager = context.GetSystemService(Context.ClipboardService) as ClipboardManager;
                ClipData clip = ClipData.NewPlainText("label", GetCodeGuard());
                clipboardManager.PrimaryClip = clip;

                Toast.MakeText(context, "Copy!", ToastLength.Short).Show();
                
            }
        }
        public override void OnEnabled(Context context)
        {
            base.OnEnabled(context);
            if(GetCodeGuard() == null)
                Toast.MakeText(context, "Account not found!", ToastLength.Long).Show();
            else
            {
                RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.guardwidget);
                views.SetTextViewText(Resource.Id.AccauntName, $"Logged in as user {steamGuard.AccountName}");

                ComponentName appWidgets = new ComponentName(context, Java.Lang.Class.FromType(typeof(GuardWidgetClass)));
                AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
                int[] ids = appWidgetManager.GetAppWidgetIds(appWidgets);

                appWidgetManager.UpdateAppWidget(ids, views);

                StartRepeating(context);
            }
        }

        public override void OnDisabled(Context context)
        {
            base.OnDisabled(context);
            //Stop Repeating
            AlarmManager alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Cancel(createPendingIntent(context, GuardUpDate));
        }

        private void StartRepeating(Context context)
        {
            AlarmManager alarm = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarm.SetRepeating(AlarmType.Rtc, SystemClock.ElapsedRealtime(), 30000, createPendingIntent(context, GuardUpDate));
        }
        private PendingIntent createPendingIntent(Context context, string action) => PendingIntent.GetBroadcast(context, 0, new Intent(context, Java.Lang.Class.FromType(typeof(GuardWidgetClass))).SetAction(action), PendingIntentFlags.Mutable);
        
        private string GetCodeGuard()
        {
            try
            {
                if (steamGuard == null)
                {
                    List<string> files = IO.Files;
                    steamGuard = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(files[0]));
                }
                return steamGuard.GenerateSteamGuardCode();
            }
            catch 
            {
                steamGuard = null;
                return null;
            }

        }
    }
}