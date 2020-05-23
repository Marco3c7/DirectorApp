using DirectorApp.Models;
using DirectorApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DirectorApp.ViewModels
{
    public class MensajesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private List<Models.Mensaje> mensajes;
        public List<Models.Mensaje> Mensajes
        {
            get { return mensajes; }
            set { mensajes = value; OnPropertyChanged(); }
        }

        private bool cargando;

        public bool Cargando
        {
            get { return cargando; }
            set { cargando = value; OnPropertyChanged(); }
        }

        private string mensaje;

        public string Mensaje
        {
            get { return mensaje; }
            set { mensaje = value; OnPropertyChanged(); }
        }

        private bool visible;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; OnPropertyChanged(); }
        }

        public Models.Mensaje AvisoGeneral { get; set; }

        public Command DescargarCommand { get; set; }
        public Command<int> VerDatosCommand { get; set; }
        public Command<DateTime> FiltrarCommand { get; set; }
        public Command MostrarMensajesCommand { get; set; }
        public Command AbrirAgregarCommand { get; set; }
        public Command EnviarMensajeCommand { get; set; }
        public MensajesViewModel()
        {
            Mensajes = new List<Models.Mensaje>();
            DescargarCommand = new Command(DescargarMensajes);
            VerDatosCommand = new Command<int>(VerDatosMensaje);
            FiltrarCommand = new Command<DateTime>(FiltarByFecha);
            MostrarMensajesCommand = new Command(VerAvisos);
            AbrirAgregarCommand = new Command(AbrirAgregar);
            EnviarMensajeCommand = new Command(EnviarMensaje);
        }

        private async void EnviarMensaje(object obj)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                    if (AvisoGeneral != null)
                    {
                        if (string.IsNullOrWhiteSpace(AvisoGeneral.Titulo))
                        {
                            throw new Exception("Escriba el título para el aviso");
                        }
                        if (string.IsNullOrWhiteSpace(AvisoGeneral.Contenido))
                        {
                            throw new Exception("El contenido del aviso no debe estar vacío.");
                        }

                        HttpClient client = new HttpClient();

                        var aviso = new Dictionary<string, string>()
                    {
                            {"Titulo", AvisoGeneral.Titulo},
                            {"Contenido",AvisoGeneral.Contenido },
                            {"FechaEnviar",DateTime.Today.Date.ToShortDateString() },
                            {"FechaCaducidad",DateTime.Today.Date.AddDays(2).ToShortDateString() },
                            {"idEscuela",  App.AvisosPrimaria.GetDatosEscuela().IdEscuela.ToString()},
                            {"FechaEnviado",DateTime.Today.Date.AddDays(1).ToShortDateString()}
                    };
                        var resp = await client.PostAsync("http://avisosprimaria.itesrc.net/api/AdminApp/enviaravisogeneral", new FormUrlEncodedContent(aviso));
                        var s = await resp.Content.ReadAsStringAsync();
                        if (resp.IsSuccessStatusCode)
                        {
                            App.AvisosPrimaria.ShowSnackBar("El aviso se envíado correctamente.");
                            await App.Current.MainPage.Navigation.PopAsync();
                            DescargarMensajes();
                        }
                        else
                        {
                            var error = await resp.Content.ReadAsStringAsync();
                            if (string.IsNullOrWhiteSpace(error))
                            {
                                throw new Exception("Ha ocurrido un error con el servidor, intente más tarde.");
                            }
                            else
                            {
                                throw new Exception(error);
                            }
                        }
                    }
                    else
                    {
                        App.AvisosPrimaria.ShowToast("Escriba el aviso.");
                    }
                }
                catch (Exception ex)
                {
                    Visible = false;
                    Cargando = false;
                    Mensaje = "";
                    App.AvisosPrimaria.ShowToast(ex.Message);
                }
            }
            else
            {
                App.AvisosPrimaria.ShowToast("Error de conexión de red");
            }
        }

        AgregarMensaje agregarPage;
        private async void AbrirAgregar()
        {
            if (agregarPage == null)
            {
                agregarPage = new AgregarMensaje();
            }
            AvisoGeneral = new Models.Mensaje();
            agregarPage.BindingContext = null;
            agregarPage.BindingContext = this;
            await App.Current.MainPage.Navigation.PushAsync(agregarPage);
        }

        private void VerAvisos()
        {
            Mensajes = App.AvisosPrimaria.Connection.Table<Models.Mensaje>().ToList();
        }

        private void FiltarByFecha(DateTime obj)
        {
            Mensajes = App.AvisosPrimaria.Connection.Table<Models.Mensaje>().Where(x=>x.FechaCaducidad==obj||x.FechaEnviado==obj).ToList();
        }

        Views.Mensaje pageMensaje;
        private async void VerDatosMensaje(int obj)
        {
            if (pageMensaje == null)
            {
                pageMensaje = new Views.Mensaje();
            }
            AvisoGeneral = App.AvisosPrimaria.Connection.Table<Models.Mensaje>().FirstOrDefault(x=>x.IdAvisosGenerales == obj);
            pageMensaje.BindingContext = null;
            pageMensaje.BindingContext = AvisoGeneral;
            await App.Current.MainPage.Navigation.PushAsync(pageMensaje);
        }

        private async void DescargarMensajes()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";

                    var escuela = App.AvisosPrimaria.GetDatosEscuela();
                    if (escuela != null)
                    {
                        HttpClient client = new HttpClient();
                        var resp = await client.GetAsync($"http://avisosprimaria.itesrc.net/api/avisosgenerales/nombreescuela/{escuela.Nombre}");
                        if (resp.IsSuccessStatusCode)
                        {
                            Mensajes = JsonConvert.DeserializeObject<List<Models.Mensaje>>(await resp.Content.ReadAsStringAsync());
                            App.AvisosPrimaria.Connection.DeleteAll<Models.Mensaje>();
                            if (Mensajes.Count > 0)
                            {
                                foreach (var item in Mensajes)
                                {
                                    if (!App.AvisosPrimaria.Connection.Table<Models.Mensaje>().Any(y => y.IdAvisosGenerales == item.IdAvisosGenerales))
                                    {
                                        App.AvisosPrimaria.Connection.Insert(item);
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(await resp.Content.ReadAsStringAsync());
                        }
                    }
                    else
                    {
                        throw new Exception("No se encontraron los mensajes datos de la escuela.");
                    }

                    Cargando = false;
                    Mensaje = "Listo";
                    Visible = false;
                }
                catch (Exception ex)
                {
                    Cargando = false;
                    App.AvisosPrimaria.ShowToast(ex.Message);
                    Visible = false;
                }
            }
            else
            {
                Mensajes = App.AvisosPrimaria.Connection.Table<Models.Mensaje>().ToList();
                Cargando = false;
                App.AvisosPrimaria.ShowToast("Error de conexión de red");
                Visible = false;
            }
        }
    }
}
