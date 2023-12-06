using System;

namespace Boletaje.Models
{
    public class HistoricoDetalladoViewModel
    {
        public historicoDet[] Historico { get; set; }

        public class historicoDet
        {
            public string Boleta { get; set; }
            public DateTime Fecha { get; set; }
            public string Tecnico { get; set; }
            public string DocEntryEntrega { get; set; }
            public string Articulo { get; set; }
            public string Descripcion { get; set; }
            public decimal Garantia { get; set; }
            public decimal Facturado { get; set; }

        }
    }
}
