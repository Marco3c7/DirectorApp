using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;

namespace DirectorApp.Droid
{
    [Activity(Label = "DirectorApp", Icon = "@mipmap/icon", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        // Iniciar la tarea
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Evitar que el botón de atrás cancele el inicio
        public override void OnBackPressed() { }

        // Simular el inicio
        async void SimulateStartup()
        {
            Log.Debug(TAG, "Realizar un trabajo de inicio que lleva un poco de tiempo.");
            // Simular la tarea de inicio en milisegundos.
            await Task.Delay(3000);
            Log.Debug(TAG, "El Splash ha finalizado - iniciando MainActivity.");
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}