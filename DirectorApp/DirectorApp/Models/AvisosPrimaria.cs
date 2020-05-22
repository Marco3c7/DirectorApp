using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DirectorApp.Models
{
    public class AvisosPrimaria
    {
        public SQLiteConnection Connection { get; set; }
        readonly string ruta = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/directorapp";

        public AvisosPrimaria()
        {
            Connection = new SQLiteConnection(ruta);
            Connection.CreateTable<Escuela>();
            Connection.CreateTable<Grupo>();
            Connection.CreateTable<Maestro>();
            Connection.CreateTable<Alumno>();
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
        }
    }
}
