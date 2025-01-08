using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class EncReparacionViewModel
    {
        public int id { get; set; }
        public int idLlamada { get; set; }
        public int idTecnico { get; set; }
        public int idDiagnostico { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int TipoReparacion { get; set; }
        public string idProductoArreglar { get; set; }
        public string SerieFabricante { get; set; }
        public int Status { get; set; }
        public int TipoCaso { get; set; }
        public bool ProcesadaSAP { get; set; }
        public string Comentarios { get; set; }
        public string BodegaOrigen { get; set; }
        public string BodegaFinal { get; set; }
        public int StatusLlamada { get; set; }
        public DateTime FechaSISO { get; set; }
        public string PrioridadAtencion { get; set; }
        public DetReparacionViewModel[] Detalle { get; set; }
        public AdjuntosViewModel[] Adjuntos { get; set; }
        public AdjuntosIdentificacionViewModel[] AdjuntosIdentificacion { get; set; }

    }
}
