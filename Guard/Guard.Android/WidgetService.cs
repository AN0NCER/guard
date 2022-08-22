using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Guard.Library;
using Newtonsoft.Json;
using SteamAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Guard.Droid
{
    [Service]
    public class WidgetService : Service
    {
        private SteamGuardAccount steamGuard;
        public override void OnStart(Intent intent, int startId)
        {
            // Build the widget update for today
            RemoteViews updateViews = buildUpdate(this);

            // Push update for this widget to the home screen
            ComponentName thisWidget = new ComponentName(this, Java.Lang.Class.FromType(typeof(GuardWidgetClass)).Name);//"guard.GuardWidgetClass"
            AppWidgetManager manager = AppWidgetManager.GetInstance(this);
            manager.UpdateAppWidget(thisWidget, updateViews);
        }

        public override IBinder OnBind(Intent intent)
		{
			// We don't need to bind to this service
			return null;
		}

        //Build a widget update to show the current Wiktionary
         //"Word of the day." Will block until the online API returns.
        public RemoteViews buildUpdate(Context context)
        {

            if (steamGuard == null)
            {
                List<string> files = IO.Files;
                steamGuard = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(files[0]));
            }

            string code = steamGuard.GenerateSteamGuardCode();

            RemoteViews views = new RemoteViews(this.PackageName, Resource.Layout.guardwidget);
            views.SetTextViewText(Resource.Id.Code, code);

            return views;
        }

    }
}