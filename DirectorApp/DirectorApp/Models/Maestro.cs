using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorApp.Models
{
    public class Maestro
    {
        [PrimaryKey]
        public int IdMaestro { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public int IdEscuela { get; set; }
        public string Password { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
