using DirectorApp.Models;
using DirectorApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DirectorApp.ViewModels
{
    public class AlumnosViewModel : INotifyPropertyChanged
    {
        private List<Alumno> listaAlumnos;

        public List<Alumno> ListaAlumnos
        {
            get { return listaAlumnos; }
            set { listaAlumnos = value; OnPropertyChanged(); }
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

        private int totalAlumnos;

        public int TotalAlumnos
        {
            get { return App.AvisosPrimaria.Connection.Table<Alumno>().Count(); }
            set { totalAlumnos = value; OnPropertyChanged(); }
        }

        private bool confirmarContraseñaEnabled;

        public bool ConfirmarContraseñaEnabled
        {
            get { return confirmarContraseñaEnabled; }
            set { confirmarContraseñaEnabled = value; OnPropertyChanged(); }
        }


        public Escuela Escuela { get; set; }

        public string Buscar { get; set; }
        public Command<string> BuscarCommand { get; set; }
        public Command AbrirAgregarAlumnoCommand { get; set; }
        public Command CancelarCommand { get; set; }
        public Command AgregarAlumnoCommand { get; set; }
        public Command EditarAlumnoCommand { get; set; }
        public Command<int> AbrirEditarAlumnoCommand { get; set; }
        public Command<bool> HabilitarBotonCommand { get; set; }
        public Command DescargarCommand { get; set; }
        public Command<int> EliminarCommand { get; set; }
        //public Command<int> VerDatosAlumnoCommand { get; set; }

        public AlumnosViewModel()
        {
            AbrirAgregarAlumnoCommand = new Command(AbrirAgregar);
            AbrirEditarAlumnoCommand = new Command<int>(AbrirEditar);
            DescargarCommand = new Command(Descargar);
            BuscarCommand = new Command<string>(Filtrar);
            ListaAlumnos = new List<Alumno>();
            Escuela = App.AvisosPrimaria.GetDatosEscuela();
            AgregarAlumnoCommand = new Command(AgregarAlumno);
            EditarAlumnoCommand = new Command(EditarAlumno);
            CancelarCommand = new Command(Cancelar);
            HabilitarBotonCommand = new Command<bool>(HabilitarBoton);
            EliminarCommand = new Command<int>(EliminarAlumno);
            //VerDatosAlumnoCommand = new Command<int>(VerDatosAlumno);
        }

        //async void VerDatosAlumno(int obj)
        //{
        //    Alumno = App.AvisosPrimaria.Connection.Table<Alumno>().Where(x => x.IdAlumno == obj).FirstOrDefault();
        //}

        async void EliminarAlumno(int obj)
        {
            try
            {
                Alumno a = new Alumno();
                a = App.AvisosPrimaria.Connection.Table<Alumno>().Where(x => x.IdAlumno == obj).FirstOrDefault();
                if (a != null)
                {
                    bool answer = await App.Current.MainPage.DisplayAlert("Eliminar Alumno", $"¿Está seguro que desea eliminar al alumno: {a.Nombre}", "Aceptar", "Cancelar");
                    if (answer)
                    {
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                        {
                            Visible = true;
                            Cargando = true;
                            Mensaje = "Eliminando...";
                            HttpClient client = new HttpClient();
                            var resp = await client.DeleteAsync($"http://avisosprimaria.itesrc.net/api/AdminApp/deletealumno?idAlumno={a.IdAlumno}");
                            if (resp.IsSuccessStatusCode)
                            {
                                Visible = false;
                                Cargando = false;
                                Mensaje = "";
                                App.AvisosPrimaria.Connection.Delete(a);
                                await App.Current.MainPage.DisplayAlert("Eliminar Alumno", $"El alumno: {a.Nombre}, se ha eliminado correctamente?", "Aceptar");
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

        void HabilitarBoton(bool obj)
        {
            ConfirmarContraseñaEnabled = obj;
        }

        private void Filtrar(string obj)
        {
            if (!string.IsNullOrWhiteSpace(obj))
            {
                ListaAlumnos = App.AvisosPrimaria.Connection.Table<Alumno>().Where(x => x.Nombre.ToUpper().Contains(obj.ToUpper())).ToList();
            }
            else
            {
                ListaAlumnos = App.AvisosPrimaria.Connection.Table<Alumno>().ToList();
            }
        }

        async void Descargar()
        {
            await GetAlumnos();
        }

        async Task GetAlumnos()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                    MensajeError = "";
                    var escuela = App.AvisosPrimaria.GetDatosEscuela();
                    if (escuela != null)
                    {
                        HttpClient client = new HttpClient();
                        var resp = await client.GetAsync($"http://avisosprimaria.itesrc.net/api/AdminApp/getalumnos/{escuela.IdEscuela}");
                        if (resp.IsSuccessStatusCode)
                        {
                            ListaAlumnos = JsonConvert.DeserializeObject<List<Alumno>>(await resp.Content.ReadAsStringAsync());

                            if (listaAlumnos.Count > 0)
                            {
                                foreach (var item in ListaAlumnos)
                                {
                                    if (!App.AvisosPrimaria.Connection.Table<Alumno>().Any(y => y.IdAlumno == item.IdAlumno))
                                    {
                                        App.AvisosPrimaria.Connection.Insert(item);
                                    }
                                }
                            }
                            TotalAlumnos = App.AvisosPrimaria.Connection.Table<Alumno>().Count();
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
                ListaAlumnos = App.AvisosPrimaria.Connection.Table<Alumno>().ToList();
                Cargando = false;
                Mensaje = "Error de conexión de red";
                Visible = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Alumno Alumno { get; set; }
        public string ContraseñaConfirmada { get; set; }

        private string mensajeError;

        public string MensajeError
        {
            get { return mensajeError; }
            set { mensajeError = value; OnPropertyChanged(); }
        }

        AgregarAlumno agregarPage;
        void AbrirAgregar()
        {
            if(agregarPage == null)
            {
                agregarPage = new AgregarAlumno();
            }
            Alumno = new Alumno();
            MensajeError = "";
            ContraseñaConfirmada = "";
            agregarPage.BindingContext = null;
            agregarPage.BindingContext = this;
            App.Current.MainPage.Navigation.PushAsync(agregarPage);
        }

        async void AgregarAlumno()
        {

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                        MensajeError = "";
                    if (Alumno != null)
                    {
                        Validar(Alumno, false);

                        HttpClient client = new HttpClient();
                        Alumno.IdEscuela = App.AvisosPrimaria.GetDatosEscuela().IdEscuela;

                        var alumno = new Dictionary<string, string>()
                    {
                        {"Clave",Alumno.Clave },
                        {"Nombre", Alumno.Nombre },
                        {"Password", Alumno.Password },
                        {"idEscuela", Alumno.IdEscuela.ToString() }
                    };
                        var resp = await client.PostAsync("http://avisosprimaria.itesrc.net/api/AdminApp/addalumno/", new FormUrlEncodedContent(alumno));
                        if (resp.IsSuccessStatusCode)
                        {
                            await App.Current.MainPage.DisplayAlert("Agregar Alumno", "El alumno se ha registrado correctamente.", "Aceptar");
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
        async void Cancelar()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        void Validar(Alumno a, bool editar)
        {
            if (string.IsNullOrWhiteSpace(a.Clave))
            {
                throw new Exception("Es necesario escribir una clave escolar.");
            }
            if (string.IsNullOrWhiteSpace(a.Nombre))
            {
                throw new Exception("Escriba el nombre del alumno.");
            }
            if(editar == false)
            {
                if (string.IsNullOrWhiteSpace(a.Password))
                {
                    throw new Exception("Escriba una contraseña para el alumno.");
                }
            }

            if(!string.IsNullOrWhiteSpace(a.Password))
            {
                if (string.IsNullOrWhiteSpace(ContraseñaConfirmada))
                {
                    throw new Exception("Confirme la contraseña.");
                }
                if (ContraseñaConfirmada != a.Password)
                {
                    throw new Exception("Las contraseñas no coinciden.");
                }
            }
        }

        EditarAlumno editarPage;
        void AbrirEditar(int id)
        {
            if (editarPage == null)
            {
                editarPage = new EditarAlumno();
            }
            Alumno = ListaAlumnos.Where(x => x.IdAlumno == id).FirstOrDefault();
            if (Alumno != null)
            {
                MensajeError = "";
                ContraseñaConfirmada = "";
                editarPage.BindingContext = null;
                editarPage.BindingContext = this;
                App.Current.MainPage.Navigation.PushAsync(editarPage);
            }
        }

        async void EditarAlumno()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                    if (Alumno != null)
                    {
                        Validar(Alumno, true);

                        HttpClient client = new HttpClient();
                        Alumno.IdEscuela = App.AvisosPrimaria.GetDatosEscuela().IdEscuela;

                        var alumno = new Dictionary<string, string>()
                    {
                                                        {"IdAlumno",Alumno.IdAlumno.ToString() },
                        {"Clave",Alumno.Clave },
                        {"Nombre", Alumno.Nombre },
                        {"Password", Alumno.Password },
                        {"idEscuela", Alumno.IdEscuela.ToString() }
                    };
                        var resp = await client.PostAsync("http://avisosprimaria.itesrc.net/api/AdminApp/updatealumno/", new FormUrlEncodedContent(alumno));
                        if (resp.IsSuccessStatusCode)
                        {
                            await App.Current.MainPage.DisplayAlert("Editar Alumno", "El alumno se ha editado correctamente.", "Aceptar");
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
    }
}
