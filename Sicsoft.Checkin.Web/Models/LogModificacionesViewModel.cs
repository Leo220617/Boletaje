using Microsoft.VisualBasic;
using System;

namespace Boletaje.Models
{
    public class LogModificacionesViewModel
    {
        public int id { get; set; }
        public int idLlamada { get; set; }
        public int idUsuario { get; set; }
        public string Usuario { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
