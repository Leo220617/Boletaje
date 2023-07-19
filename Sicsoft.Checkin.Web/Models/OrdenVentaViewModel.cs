using System;
using System.Collections.Generic;

namespace Boletaje.Models
{
    public class OrdenVentaViewModel
    {
        public int id { get; set; }
        public int idCondPago { get; set; }
        public int idGarantia { get; set; }
        public int idTiemposEntregas { get; set; }
        public int idDiasValidos { get; set; }
        public int idUsuarioCreador { get; set; }

        public int BaseEntry { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public string Cliente { get; set; }
        public string Moneda { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaEntrega { get; set; }

        public string TipoDocumento { get; set; }
        public string NumAtCard { get; set; }
        public int Series { get; set; }
        public string Comentarios { get; set; }
        public int CodVendedor { get; set; }
        public bool ProcesadaSAP { get; set; }
        public string PersonaContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string CorreoContacto { get; set; }
        public List<Detalle> Detalle { get; set; }

    }

    public class Detalle
    {
        public int id { get; set; }
        public int idEncabezado { get; set; }
        public int NumLinea { get; set; }
        public string ItemCode { get; set; }
        public string Bodega { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal Cantidad { get; set; }
        public string Impuesto { get; set; }
        public bool TaxOnly { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string PrecioUnitario1 { get; set; }
        public decimal Total { get; set; }
    }

}
