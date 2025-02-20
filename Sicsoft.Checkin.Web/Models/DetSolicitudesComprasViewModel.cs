namespace Boletaje.Models
{
    public class DetSolicitudesComprasViewModel
    {
        public int id { get; set; }
        public int idEncabezado { get; set; }
        public string ItemCode { get; set; }
        public int idProducto { get; set; }
        public decimal Cantidad { get; set; }
    }
}
