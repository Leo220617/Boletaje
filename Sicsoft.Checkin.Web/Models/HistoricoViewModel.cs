using System;

namespace Boletaje.Models
{
    public class HistoricoViewModel
    {

        public historico[] Historico { get; set; }

        public class historico
        {
            public string Factura { get; set; }
            public DateTime Fecha_Factura { get; set; }
            public decimal Total_Revisiones { get; set; }
            public DateTime fecha_Ultima_Rev { get; set; }
            public decimal Cantidad_Revisiones { get; set; }
            public string Ultimo_Tecnico { get; set; }
            public string Obse_Ultima_Repara { get; set; }
        }
    }
}
