﻿namespace Boletaje.Models
{
    public class CuentasBancariasViewModel
    {
        public int id { get; set; }
        public int idSucursal { get; set; }
        public string Nombre { get; set; }
        public string CuentaSAP { get; set; }
        public bool Estado { get; set; }
        public string Banco { get; set; }
        public string Moneda { get; set; }
        public string Tipo { get; set; }
    }
}
