using System;

namespace Boletaje.Models
{
    public class TipoCambiosViewModel
    {
        public int id { get; set; }
        public decimal TipoCambio { get; set; }
        public string Moneda { get; set; }
        public DateTime Fecha { get; set; }
    }
}
