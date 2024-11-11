namespace Boletaje.Models
{
    public class ProductosFacturacionInicialViewModel
    {
        public productosFacturar[] ProductosFacturar { get; set; }
    }
    public class productosFacturar
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        public decimal Stock { get; set; }

        public decimal Precio { get; set; }


    }
}
