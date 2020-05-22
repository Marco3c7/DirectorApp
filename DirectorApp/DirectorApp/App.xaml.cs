using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DirectorApp.Views;
using DirectorApp.Models;

namespace DirectorApp
{
    public partial class App : Application
    {
        public static AvisosPrimaria AvisosPrimaria { get; set; } = new AvisosPrimaria();
        public App()
        {
            InitializeComponent();
            if (AvisosPrimaria.Connection.Table<Escuela>().Count() != 0)
            {
                MainPage = new MainPage() { BackgroundColor = Color.White};
            }
            else
            {
                MainPage = new Login() { BackgroundColor = Color.White };
            }
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
