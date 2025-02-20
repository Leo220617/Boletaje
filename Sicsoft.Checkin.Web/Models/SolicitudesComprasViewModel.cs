using iTextSharp.text;
using System;

namespace Boletaje.Models
{
    public class SolicitudesComprasViewModel
    {
        public int id { get; set; }
        public string CardCode { get; set; }
        public int idOfertaAprobada { get; set; }
        public int idEncabezadoBitacora { get; set; }
        public DateTime Fecha { get; set; }
        public string GrupoArticulo { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public bool ProcesadaSAP { get; set; }
        public DetSolicitudesComprasViewModel[] Detalle { get; set; }

    }
}
