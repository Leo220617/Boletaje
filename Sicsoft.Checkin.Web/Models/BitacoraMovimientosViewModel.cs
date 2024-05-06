using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class BitacoraMovimientosViewModel
    {

        public int id { get; set; }
        public int idLlamada { get; set; }
        public int idTecnico { get; set; }
        public int idEncabezado { get; set; }
        public int DocEntry { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoMovimiento { get; set; }
        public string BodegaInicial { get; set; }
        public string BodegaFinal { get; set; }
        public string Status { get; set; }
        public bool ProcesadaSAP { get; set; }
        public int StatusLlamada { get; set; }
        public DetBitacoraMovimientosViewModel[] Detalle { get; set; }
    }
}
