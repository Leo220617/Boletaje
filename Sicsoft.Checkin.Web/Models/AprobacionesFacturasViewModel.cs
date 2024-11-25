using System;

namespace Boletaje.Models
{
    public class AprobacionesFacturasViewModel
    {
        public int id { get; set; }
        public string CardCode { get; set; }
        public string ItemCode { get; set; }
        public string Serie { get; set; }
        public bool Aprobada { get; set; }
        public DateTime Fecha { get; set; }
        public int idLoginAprobador { get; set; }
        public DateTime FechaAprobacion { get; set; }
    }
}
