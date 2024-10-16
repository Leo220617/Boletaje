namespace Boletaje.Models
{
    public class ExoneracionesViewModel
    {
        public exoneracion[] Exoneraciones { get; set; }
    }
    public class exoneracion
    {
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Emisora { get; set; }
        public string FechaEmision { get; set; }
        public string CodTarifa { get; set; }
        public string Prcnt { get; set; }
        public string ItemCode { get; set; }
        public string CardCode { get; set; }
        public string DocEntry { get; set; }
    }
}
