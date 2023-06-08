namespace Boletaje.Models
{
    public class CotizacionesAprobadasViewModel
    {
        public int id { get; set; }
        public int idEncabezado { get; set; }
        public string ItemCode { get; set; }
        public decimal Cantidad { get; set; }
        public bool Opcional { get; set; }
    }
}
