using DirectorApp.Models;
using DirectorApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DirectorApp.ViewModels
{
    public class MaestrosViewModel : INotifyPropertyChanged
    {
        private List<Models.Maestro> listaMaestros;

        public List<Models.Maestro> ListaMaestros
        {
            get { return listaMaestros; }
            set { listaMaestros = value; OnPropertyChanged(); }
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

        private int totalMaestros;

        public int TotalMaestros
        {
            get { return App.AvisosPrimaria.Connection.Table<Models.Maestro>().Count(); }
            set { totalMaestros = value; OnPropertyChanged(); }
        }

        private bool confirmarContraseñaEnabled;

        public bool ConfirmarContraseñaEnabled
        {
            get { return confirmarContraseñaEnabled; }
            set { confirmarContraseñaEnabled = value; OnPropertyChanged(); }
        }

        private string mensajeError;

        public string MensajeError
        {
            get { return mensajeError; }
            set { mensajeError = value; OnPropertyChanged(); }
        }

        private Models.Maestro maestro;

        public Models.Maestro Maestro
        {
            get { return maestro; }
            set { maestro = value; OnPropertyChanged(); }
        }


        public string Buscar { get; set; }
        public string ContraseñaConfirmada { get; set; }
        public Command<string> BuscarCommand { get; set; }
        public Command DescargarCommand { get; set; }
        public Command CancelarCommand { get; set; }
        public Command AbrirAgregarMaestroCommand { get; set; }
        public Command AgregarMaestroCommand { get; set; }
        public Command<bool> HabilitarBotonCommand { get; set; }
        public Command<int> EliminarCommand { get; set; }
        public Command<int> VerCommand { get; set; }
        public Command EditarMaestroCommand { get; set; }
        public Escuela Escuela { get; set; }

        public Command<int> AbrirEditarCommand { get; set; }
        public MaestrosViewModel()
        {
            DescargarCommand = new Command(Descargar);
            BuscarCommand = new Command<string>(Filtrar);
            ListaMaestros = new List<Models.Maestro>();
            AbrirAgregarMaestroCommand = new Command(AbrirAgregarMaestro);
            CancelarCommand = new Command(Cancelar);
            Escuela = App.AvisosPrimaria.GetDatosEscuela();
            AgregarMaestroCommand = new Command(AgregarMaestro);
            HabilitarBotonCommand = new Command<bool>(HabilitarBoton);
            EliminarCommand = new Command<int>(EliminarMaestro);
            VerCommand = new Command<int>(VerDatosAlumno);
            AbrirEditarCommand = new Command<int>(AbrirEditar);
            EditarMaestroCommand = new Command(EditarMaestro);
        }

        async void EditarMaestro()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                    MensajeError = "";
                    if (Maestro != null)
                    {
                        Validar(Maestro, true);

                        HttpClient client = new HttpClient();
                        Maestro.IdEscuela = App.AvisosPrimaria.GetDatosEscuela().IdEscuela;

                        var maestro = new Dictionary<string, string>()
                    {
                        {"idMaestro",Maestro.IdMaestro.ToString() },
                            {"Clave", Maestro.Clave },
                            {"Nombre", Maestro.Nombre },
                            {"Password", Maestro.Password },
                            {"idEscuela", Maestro.IdEscuela.ToString() }
                    };
                        var resp = await client.PostAsync("http://avisosprimaria.itesrc.net/api/AdminApp/updatemaestro/", new FormUrlEncodedContent(maestro));
                        if (resp.IsSuccessStatusCode)
                        {
                            App.AvisosPrimaria.Connection.Update(Maestro);
                            App.AvisosPrimaria.ShowSnackBar("El maestro se ha editado correctamente.");
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
                        MensajeError = "Escriba los datos del alumno";
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

        EditarMaestro editarPage;
        private void AbrirEditar(int obj)
        {
            if (editarPage == null)
            {
                editarPage = new EditarMaestro();
            }
            Maestro = App.AvisosPrimaria.Connection.Table<Models.Maestro>().Where(x => x.IdMaestro == obj).FirstOrDefault();
            MensajeError = "";
            ContraseñaConfirmada = "";
            editarPage.BindingContext = null;
            editarPage.BindingContext = this;
            App.Current.MainPage.Navigation.PushAsync(editarPage);
        }

        Views.Maestro datosPage;
        private async void VerDatosAlumno(int obj)
        {
            if (datosPage == null)
            {
                datosPage = new Views.Maestro();
            }
            Maestro = App.AvisosPrimaria.Connection.Table<Models.Maestro>().Where(x => x.IdMaestro == obj).FirstOrDefault();
            Escuela = App.AvisosPrimaria.GetDatosEscuela();
            datosPage.BindingContext = null;
            datosPage.BindingContext = this;
            await App.Current.MainPage.Navigation.PushAsync(datosPage);
        }

        private async void EliminarMaestro(int obj)
        {
            try
            {
                Models.Maestro m = new Models.Maestro();
                m = App.AvisosPrimaria.Connection.Table<Models.Maestro>().Where(x => x.IdMaestro == obj).FirstOrDefault();
                if (m != null)
                {
                    bool answer = await App.Current.MainPage.DisplayAlert("Eliminar Maestro", $"¿Está seguro que desea eliminar al maestro: {m.Nombre}?", "Aceptar", "Cancelar");
                    if (answer)
                    {
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                        {
                            Visible = true;
                            Cargando = true;
                            Mensaje = "Eliminando...";
                            HttpClient client = new HttpClient();
                            var resp = await client.DeleteAsync($"http://avisosprimaria.itesrc.net/api/AdminApp/deletemaestro?idMaestro={m.IdMaestro}");
                            if (resp.IsSuccessStatusCode)
                            {
                                Visible = false;
                                Cargando = false;
                                Mensaje = "";
                                App.AvisosPrimaria.Connection.Delete(m);
                                App.AvisosPrimaria.ShowSnackBar($"El maestro: { m.Nombre}, se ha eliminado correctamente");
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

        private void HabilitarBoton(bool obj)
        {
            ConfirmarContraseñaEnabled = obj;
        }

        async void AgregarMaestro()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                    MensajeError = "";
                    if (Maestro != null)
                    {
                        Validar(Maestro, false);

                        HttpClient client = new HttpClient();
                        Maestro.IdEscuela = Escuela.IdEscuela;

                        var maestro = new Dictionary<string, string>()
                    {
                            {"Clave",Maestro.Clave },
                            {"Nombre",Maestro.Nombre },
                            {"Password",Maestro.Password },
                            {"idEscuela",Maestro.IdEscuela.ToString() }
                    };
                        var resp = await client.PostAsync("http://avisosprimaria.itesrc.net/api/AdminApp/addmaestro/", new FormUrlEncodedContent(maestro));
                        if (resp.IsSuccessStatusCode)
                        {
                            await App.Current.MainPage.Navigation.PopAsync();
                            App.AvisosPrimaria.ShowSnackBar("El maestro se ha registrado correctamente.");
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

        private async void Cancelar()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        AgregarMaestro agregarPage;
        private async void AbrirAgregarMaestro()
        {
            if(agregarPage == null)
            {
                agregarPage = new AgregarMaestro();
            }
            Maestro = new Models.Maestro();
            MensajeError = "";
            ContraseñaConfirmada = "";
            agregarPage.BindingContext = null;
            agregarPage.BindingContext = this;
            await App.Current.MainPage.Navigation.PushAsync(agregarPage);
        }

        private void Filtrar(string obj)
        {
            if (!string.IsNullOrWhiteSpace(obj))
            {
                ListaMaestros = App.AvisosPrimaria.Connection.Table<Models.Maestro>().Where(x => x.Clave.Contains(obj) || x.Nombre.ToUpper().Contains(obj.ToUpper())).ToList();
            }
            else
            {
                ListaMaestros = App.AvisosPrimaria.Connection.Table<Models.Maestro>().ToList();
            }
        }

        async void Descargar()
        {
            await GetMaestros();
        }

        async Task GetMaestros()
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
                        var resp = await client.GetAsync($"http://avisosprimaria.itesrc.net/api/AdminApp/getmaestros/{escuela.IdEscuela}");
                        if (resp.IsSuccessStatusCode)
                        {
                            ListaMaestros = JsonConvert.DeserializeObject<List<Models.Maestro>>(await resp.Content.ReadAsStringAsync());
                            App.AvisosPrimaria.Connection.DeleteAll<Models.Maestro>();
                            if (listaMaestros.Count > 0)
                            {
                                foreach (var item in ListaMaestros)
                                {
                                    if (!App.AvisosPrimaria.Connection.Table<Models.Maestro>().Any(y => y.IdMaestro == item.IdMaestro))
                                    {
                                        App.AvisosPrimaria.Connection.Insert(item);
                                    }
                                }
                            }
                            TotalMaestros = App.AvisosPrimaria.Connection.Table<Models.Maestro>().Count();
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
                ListaMaestros = App.AvisosPrimaria.Connection.Table<Models.Maestro>().ToList();
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

        void Validar(Models.Maestro m, bool editar)
        {
            if (string.IsNullOrWhiteSpace(m.Clave))
            {
                throw new Exception("Es necesario escribir una clave escolar.");
            }
            if (string.IsNullOrWhiteSpace(m.Nombre))
            {
                throw new Exception("Escriba el nombre del maestro.");
            }
            if (editar == false)
            {
                if (string.IsNullOrWhiteSpace(m.Password))
                {
                    throw new Exception("Escriba una contraseña para el maestro.");
                }
            }

            if (!string.IsNullOrWhiteSpace(m.Password))
            {
                if (string.IsNullOrWhiteSpace(ContraseñaConfirmada))
                {
                    throw new Exception("Confirme la contraseña.");
                }
                if (ContraseñaConfirmada != m.Password)
                {
                    throw new Exception("Las contraseñas no coinciden.");
                }
            }
        }
    }
}
