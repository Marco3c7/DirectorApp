using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorApp.Models
{
    public class Escuela
    {
        [PrimaryKey]
        public int IdEscuela { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string NombreAdmin { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}
