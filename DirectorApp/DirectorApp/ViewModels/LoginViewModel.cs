using DirectorApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using DirectorApp.Views;

namespace DirectorApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public string Nombre { get; set; }
        public string Contraseña { get; set; }

        private string error;

        public string Error
        {
            get { return error; }
            set { error = value; OnPropertyChanged(); }
        }

        private bool cargando;

        public bool Cargando
        {
            get { return cargando; }
            set { cargando = value; OnPropertyChanged(); }
        }


        public Command IniciarSesion { get; set; }

        public LoginViewModel()
        {
            IniciarSesion = new Command(Iniciar);
        }

        public async void Iniciar()
        {
            await Login();
        }

        MainPage mainPage;
        async Task Login()
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    if (string.IsNullOrWhiteSpace(this.Nombre))
                    {
                        Error = "Escriba un nombre.";
                    }
                    else if (string.IsNullOrWhiteSpace(this.Contraseña))
                    {
                        Error = "Escriba una contraseña.";
                    }
                    else
                    {
                        Error = "";
                        Cargando = true;
                        HttpClient client = new HttpClient();
                        Dictionary<string, string> datos = new Dictionary<string, string>()
                        {
                            { "nombreAdmin", this.Nombre },
                            { "password", this.Contraseña}
                        };
                        var resp = await client.PostAsync("http://avisosprimaria.itesrc.net/api/AdminApp/Login", new FormUrlEncodedContent(datos));
                        if (resp.IsSuccessStatusCode)
                        {
                            EscuelaGrupos escuelaGrupos = new EscuelaGrupos();
                            escuelaGrupos = JsonConvert.DeserializeObject<EscuelaGrupos>(await resp.Content.ReadAsStringAsync());
                            Escuela e = new Escuela
                            {
                                IdEscuela = escuelaGrupos.IdEscuela,
                                Nombre = escuelaGrupos.NombreEscuela,
                                NombreAdmin = this.Nombre.ToUpper(),
                                FechaRegistro = escuelaGrupos.fechaRegistroEscuela
                            };
                            App.AvisosPrimaria.Connection.Insert(e);

                            escuelaGrupos.Grupos.ForEach(x => App.AvisosPrimaria.Connection.Insert(x));
                            escuelaGrupos.Alumnos.ForEach(x => App.AvisosPrimaria.Connection.Insert(x));
                            escuelaGrupos.Maestros.ForEach(x => App.AvisosPrimaria.Connection.Insert(x));

                            if (mainPage == null)
                            {
                                mainPage = new MainPage();
                                mainPage.BackgroundColor = Color.White;
                            }
                            Cargando = false;
                            App.Current.MainPage = mainPage;
                        }
                        else
                        {
                            Cargando = false;
                            Error = "La cuenta o la contraseña es incorrecta.";
                        }
                    }
                }
                else
                {
                    Cargando = false;
                    Error = "Error de conexión de red,  inténtelo más tarde.";
                }
            }
            catch (Exception ex)
            {
                Cargando = false;
                Error = ex.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}