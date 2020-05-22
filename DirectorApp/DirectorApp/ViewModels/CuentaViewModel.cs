using DirectorApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using DirectorApp.Views;
using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace DirectorApp.ViewModels
{
    public class CuentaViewModel : INotifyPropertyChanged
    {
        public Escuela Escuela { get; set; }

        private int totalGrupos;

        public int TotalGrupos
        {
            get { return totalGrupos; }
            set { totalGrupos = value; OnPropertyChanged(); }
        }

        private int totalMaestros;

        public int TotalMaestros
        {
            get { return totalMaestros; }
            set { totalMaestros = value; OnPropertyChanged(); }
        }

        private int totalAlumnos;

        public int TotalAlumnos
        {
            get { return totalAlumnos; }
            set { totalAlumnos = value; }
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
        private bool confirmarContraseñaEnabled;

        public bool ConfirmarContraseñaEnabled
        {
            get { return confirmarContraseñaEnabled; }
            set { confirmarContraseñaEnabled = value; OnPropertyChanged(); }
        }
        public string ContraseñaConfirmada { get; set; }

        EditarCuenta editarCuentaPage;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        string oldPassword;
        public Command AbrirEditarCommand { get; set; }
        public Command ActualizarVistaCommand { get; set; }
        public Command CancelarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command<bool> HabilitarBotonCommand { get; set; }
        public CuentaViewModel()
        {
            Escuela = App.AvisosPrimaria.GetDatosEscuela();
            TotalGrupos = App.AvisosPrimaria.TotalGrupos();
            TotalMaestros = App.AvisosPrimaria.Connection.Table<Models.Maestro>().Count();
            TotalAlumnos = App.AvisosPrimaria.Connection.Table<Models.Alumno>().Count();
            AbrirEditarCommand = new Command(AbrirEditar);
            ActualizarVistaCommand = new Command(Actualizar);
            CancelarCommand = new Command(Cancelar);
            EditarCommand = new Command(EditarCuenta);
            HabilitarBotonCommand = new Command<bool>(HabilitarBoton);
        }

        void HabilitarBoton(bool obj)
        {
            ConfirmarContraseñaEnabled = obj;
        }

        private async void EditarCuenta()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    Visible = true;
                    Cargando = true;
                    Mensaje = "Cargando...";
                    if (Escuela != null)
                    {
                        Validar();
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

        private void Actualizar()
        {
            TotalGrupos = App.AvisosPrimaria.TotalGrupos();
            TotalMaestros = App.AvisosPrimaria.Connection.Table<Models.Maestro>().Count();
            TotalAlumnos = App.AvisosPrimaria.Connection.Table<Models.Alumno>().Count();
        }

        async void AbrirEditar()
        {
            if (editarCuentaPage == null)
            {
                editarCuentaPage = new EditarCuenta();
            }
            Visible = false;
            Cargando = false;
            MensajeError = "";
            Mensaje = "";
            oldPassword = Escuela.Password;
            editarCuentaPage.BindingContext = null;
            editarCuentaPage.BindingContext = this;
            await App.Current.MainPage.Navigation.PushAsync(editarCuentaPage);
        }

        void Validar()
        {
            if (string.IsNullOrWhiteSpace(Escuela.Nombre))
            {
                throw new Exception("El nombre de la institución es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(Escuela.NombreAdmin))
            {
                throw new Exception("El nombre del Director de la institución es obligatorio.");
            }

            if (!string.IsNullOrWhiteSpace(Escuela.Password))
            {
                if (string.IsNullOrWhiteSpace(ContraseñaConfirmada))
                {
                    throw new Exception("Confirme la contraseña.");
                }
                if (Escuela.Password != ContraseñaConfirmada)
                {
                    throw new Exception("Las contraseñas no coinciden.");
                }
            }
        }


    }
}
