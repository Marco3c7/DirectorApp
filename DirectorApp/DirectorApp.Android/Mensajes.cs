using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using DirectorApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(DirectorApp.Droid.Mensajes))]
namespace DirectorApp.Droid
{
    public class Mensajes : IMessage
    {
        public void ShowSnackBar(string text)
        {
            var view = MainActivity.Actividad.FindViewById(Android.Resource.Id.Content);
            var s = Snackbar.Make(view, text, Snackbar.LengthLong);
            s.Show();
        }

        public void ShowToast(string text)
        {
            var t = Toast.MakeText(Application.Context, text, ToastLength.Long);
            t.Show();
        }
    }
}