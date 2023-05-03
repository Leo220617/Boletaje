namespace Boletaje.Models
{
    public class ProductosCOrdenesViewModel
    {

        public Productos[] Productos { get; set; }
    }

    public class Productos
    {
        public string Codigo { get; set; }
        public string idBodega { get; set; }
        public int ListaPrecio { get; set; }
        public string Nombre { get; set; }
        public string Moneda { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal StockReal { get; set; }
        public int Editable { get; set; }
    }
}
