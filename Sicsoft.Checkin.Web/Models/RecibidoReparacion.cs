using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class RecibidoReparacion
    {
        public int id { get; set; }
        public int Tipo { get; set; }
        public int Status { get; set; }
        public int StatusLlamada { get; set; }
        public int TipoCasoLlamada { get; set; }
        public int Garantia { get; set; }
        public int HorasLlamada { get; set; }
        public int idLlamada { get; set; }
        public string comentarios { get; set; }
        public string BodegaInicial { get; set; }
        public string BodegaFinal { get; set; }
        public int idDiagnostico { get; set; }

        public DetReparacionViewModel[] DetReparacion { get; set; }
        public AdjuntosViewModel[] Adjuntos { get; set; }
    }
}
