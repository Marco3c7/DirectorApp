using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorApp.Models
{
    public class Mensaje
    {
        [PrimaryKey]
        public int IdAvisosGenerales { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime? FechaEnviado { get; set; }
        public DateTime? FechaCaducidad { get; set; }

    }
}
