using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorApp.Models
{
    public class EscuelaGrupos
    {
        public int IdEscuela { get; set; }
        public string NombreEscuela { get; set; }
        public DateTime fechaRegistroEscuela { get; set; }
        public List<Grupo> Grupos { get; set; }
        public List<Maestro> Maestros { get; set; }

        public List<Alumno> Alumnos { get; set; }
    }
}
