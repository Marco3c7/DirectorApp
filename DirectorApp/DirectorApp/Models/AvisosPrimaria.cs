using DirectorApp.Interfaces;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DirectorApp.Models
{
    public class AvisosPrimaria
    {
        public SQLiteConnection Connection { get; set; }
        readonly string ruta = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/directorapp";
        IMessage msj;
        public AvisosPrimaria()
        {
            msj = DependencyService.Get<IMessage>();
            Connection = new SQLiteConnection(ruta);
            Connection.CreateTable<Escuela>();
            Connection.CreateTable<Grupo>();
            Connection.CreateTable<Maestro>();
            Connection.CreateTable<Alumno>();
            Connection.CreateTable<Mensaje>();
        }

        public Escuela GetDatosEscuela()
        {
            return Connection.Table<Escuela>().FirstOrDefault();
        }

        public List<Grupo> GetGrupos()
        {
            return new List<Grupo>(Connection.Table<Grupo>());
        }

        public int TotalGrupos()
        {
            return Connection.Table<Grupo>().Count();
        }


        public void Logout()
        {
            Connection.DeleteAll<Escuela>();
            Connection.DeleteAll<Grupo>();
            Connection.DeleteAll<Maestro>();
            Connection.DeleteAll<Alumno>();
            Connection.DeleteAll<Mensaje>();
        }

        public void ShowToast(string mensaje)
        {
            msj.ShowToast(mensaje);
        }

        public void ShowSnackBar(string mensaje)
        {
            msj.ShowSnackBar(mensaje);
        }
    }
}
