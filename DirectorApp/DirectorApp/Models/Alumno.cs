using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorApp.Models
{
    public class Alumno
    {
        [PrimaryKey]
        public int IdAlumno { get; set; }
        public string NombreEscuela { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public int IdEscuela { get; set; }
    }
}
