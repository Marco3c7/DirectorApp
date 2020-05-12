using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DirectorApp.Views;

namespace DirectorApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            MainPage = new Mensajes();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
