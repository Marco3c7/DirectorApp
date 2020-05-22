using DirectorApp.Models;
using DirectorApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DirectorApp.ViewModels
{
    public class GruposViewModel : INotifyPropertyChanged
    {
        private List<Grupo> listaGrupos;

        public List<Grupo> ListaGrupos
        {
            get { return listaGrupos; }
            set { listaGrupos = value; OnPropertyChanged(); }
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

        private string mensajeError;

        public string MensajeError
        {
            get { return mensajeError; }
            set { mensajeError = value; OnPropertyChanged(); }
        }

        public string Buscar { get; set; }
        public Command<string> BuscarCommand { get; set; }
        public Command DescargarCommand { get; set; }
        public Command AbrirAgregarCommand { get; set; }
        public Command AgregarGrupoCommand { get; set; }
        public Command CancelarCommand { get; set; }
        public Command<int> EliminarCommand { get; set; }
        public Grupo Grupo { get; set; }
        public GruposViewModel()
        {
            DescargarCommand = new Command(Descargar);
            BuscarCommand = new Command<string>(Filtrar);
            ListaGrupos = new List<Grupo>();
            AbrirAgregarCommand = new Command(AbrirAgregar);
            AgregarGrupoCommand = new Command(AgregarGrupo);
            CancelarCommand = new Command(Cancelar);
            EliminarCommand = new Command<int>(Eliminar);
        }

        private async void Eliminar(int obj)
        {
            try
            {
                Models.Grupo g = new Models.Grupo();
                g = App.AvisosPrimaria.Connection.Table<Models.Grupo>().Where(x => x.IdGrupo == obj).FirstOrDefault();
                if (g != null)
                {
                    bool answer = await App.Current.MainPage.DisplayAlert("Eliminar Grupo", $"¿Está seguro que desea eliminar al grupo de: {g.Nombre}?", "Aceptar", "Cancelar");
                    if (answer)
                    {
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                        {
                            Visible = true;
                            Cargando = true;
                            Mensaje = "Eliminando...";
                            HttpClient client = new HttpClient();
                            var resp = await client.DeleteAsync($"http://avisosprimaria.itesrc.net/api/AdminApp/deletegrupo?idGrupo={g.IdGrupo}");
                            if (resp.IsSuccessStatusCode)
                            {
                                Visible = false;
                                Cargando = false;
                                Mensaje = "";
                                App.AvisosPrimaria.Connection.Delete(g);
                                await App.Current.MainPage.DisplayAlert("Eliminar Grupo", $"El grupo: {g.Nombre}, se ha eliminado correctamente", "Aceptar");
                                Descargar();
                            }
                            else
                            {
                                throw new Exception(await resp.Content.ReadAsStringAsync());
                            }
                        }
                        else
                        {
                            throw new Exception("Error de conexión de red");
                        }
                    }
                }
                else
                {
                    throw new Exception("Seleccione el alumno que desea eliminar.");
                }

            }
            catch (Exception ex)
            {
                Visible = false;
                Cargando = false;
                Mensaje = "";
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private async void Cancelar()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        async void AgregarGrupo()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                    MensajeError = "";
                    if (Grupo != null)
                    {
                        Validar();
                        HttpClient client = new HttpClient();
                        Grupo.IdEscuela = App.AvisosPrimaria.GetDatosEscuela().IdEscuela;
                        var maestro = new Dictionary<string, string>()
                    {
                            {"Nombre",Grupo.Nombre },
                            {"idEscuela",Grupo.IdEscuela.ToString() },
                            {"CicloEscolar", Grupo.CicloEscolar }
                    };
                        var resp = await client.PostAsync("http://avisosprimaria.itesrc.net/api/AdminApp/addgrupo/", new FormUrlEncodedContent(maestro));
                        if (resp.IsSuccessStatusCode)
                        {
                            await App.Current.MainPage.DisplayAlert("Agregar Grupo", "El grupo se ha registrado correctamente.", "Aceptar");
                            await App.Current.MainPage.Navigation.PopAsync();
                            Descargar();
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
                        MensajeError = "Escriba los datos del maestro";
                    }
                }
                catch (Exception ex)
                {
                    Visible = false;
                    Cargando = false;
                    Mensaje = "";
                    MensajeError = ex.Message;
                }
            }
            else
            {
                MensajeError = "Error de conexión de red";
            }
        }

        AgregarGrupo agregarPage;
        private void AbrirAgregar()
        {
            if(agregarPage == null)
            {
                agregarPage = new AgregarGrupo();
            }
            Grupo = new Grupo();
            agregarPage.BindingContext = null;
            agregarPage.BindingContext = this;
            App.Current.MainPage.Navigation.PushAsync(agregarPage);
        }

        private void Filtrar(string obj)
        {
            if (!string.IsNullOrWhiteSpace(obj))
            {
                ListaGrupos = App.AvisosPrimaria.Connection.Table<Grupo>().Where(x => x.Nombre.ToUpper().Contains(obj.ToUpper())).ToList();
            }
            else
            {
                ListaGrupos = App.AvisosPrimaria.Connection.Table<Grupo>().ToList();
            }
        }

        async void Descargar()
        {
            await GetGrupos();
        }

        async Task GetGrupos()
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
                        var resp = await client.GetAsync($"http://avisosprimaria.itesrc.net/api/AdminApp/getgrupos/{escuela.IdEscuela}");
                        if (resp.IsSuccessStatusCode)
                        {
                            ListaGrupos = JsonConvert.DeserializeObject<List<Grupo>>(await resp.Content.ReadAsStringAsync());

                            if (listaGrupos.Count > 0)
                            {
                                foreach (var item in ListaGrupos)
                                {
                                    if (!App.AvisosPrimaria.Connection.Table<Grupo>().Any(y => y.IdGrupo == item.IdGrupo))
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
                        throw new Exception("No se encontraron los datos de la escuela.");
                    }

                    Cargando = false;
                    Mensaje = "Listo";
                    Visible = false;
                }
                catch (Exception ex)
                {
                    Cargando = false;
                    Mensaje = ex.Message;
                    await Task.Delay(1000);
                    Visible = false;
                }
            }
            else
            {
                ListaGrupos = App.AvisosPrimaria.Connection.Table<Grupo>().ToList();
                Cargando = false;
                Mensaje = "Error de conexión de red";
                await Task.Delay(TimeSpan.FromMinutes(100));
                Visible = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        void Validar()
        {
            if (string.IsNullOrWhiteSpace(Grupo.Nombre))
            {
                throw new Exception("Escriba el nombre del grupo.");
            }
            if (string.IsNullOrWhiteSpace(Grupo.CicloEscolar))
            {
                throw new Exception("Escriba el ciclo escolar.");
            }
        }

    }
}
