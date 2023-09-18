using System;

namespace Boletaje.Models
{
    public class ActividadesViewModel
    {
        public int id { get; set; }
        public int idLlamada { get; set; }
        public int DocEntry { get; set; }
        public int TipoActividad { get; set; }
        public string Detalle { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool ProcesadaSAP { get; set; }
        public int UsuarioCreador { get; set; }
    }
}
