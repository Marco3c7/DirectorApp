using DirectorApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace DirectorApp.ViewModels
{
    public class MainPageViewModel
    {
        public Command CerrarSesion { get; set; }

        public MainPageViewModel()
        {
            CerrarSesion = new Command(Logout);
        }
        async void Logout()
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Cerrar Sesión", "¿Está seguro que desea cerrar sesión?", "Aceptar", "Cancelar");

            if (answer)
            {
                App.AvisosPrimaria.Logout();
                App.Current.MainPage = new Login() { BackgroundColor=Color.White};
            }
        }
    }
}
