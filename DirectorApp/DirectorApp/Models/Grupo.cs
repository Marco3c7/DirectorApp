using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectorApp.Models
{
    public class Grupo
    {
        [PrimaryKey]
        public int IdGrupo { get; set; }
        public string Nombre { get; set; }
        public int IdEscuela { get; set; }
        public string CicloEscolar { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
