using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class EncMovimientoViewModel
    {
        public int id { get; set; }
        public int DocEntry { get; set; }

        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string NumLlamada { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoMovimiento { get; set; }
        public string Comentarios { get; set; }
        public string CreadoPor { get; set; }
        public decimal Subtotal { get; set; }
        public decimal PorDescuento { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuestos { get; set; }
        public decimal TotalComprobante { get; set; }
        public bool Generar { get; set; }
        public bool Regenerar { get; set; }

        public string Moneda { get; set; }
        public DetMovimientoViewModel[] Detalle { get; set; }
    }
}
