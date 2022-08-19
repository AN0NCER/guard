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

namespace Guard.Droid
{
    [BroadcastReceiver(Label = "GuardWidget", Exported = true)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/guardwidgetprovider")]
    public class GuardWidgetClass : AppWidgetProvider
    {
        private SteamGuardAccount steamGuard;

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            if (steamGuard == null)
            {
                List<string> files = IO.Files;
                steamGuard = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(files[0]));
            }

            while (true)
            {
                string code = steamGuard.GenerateSteamGuardCode();
                RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.guardwidget);
                views.SetTextViewText(Resource.Id.Code, code);
                appWidgetManager.UpdateAppWidget(appWidgetIds, views);
                Thread.Sleep(30000);
            }
        }
        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
        }
        
     
    }
}