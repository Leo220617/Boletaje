﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class DetBitacoraMovimientosViewModel
    {
        public int id { get; set; }
        public int idEncabezado { get; set; }
        public int idProducto { get; set; }
        public int idError { get; set; }
        public decimal Cantidad { get; set; }
        public string ItemCode { get; set; }
        public decimal CantidadEnviar { get; set; }
        public decimal CantidadFaltante { get; set; }
        public bool SolicitudCompra { get; set; }
        public bool SolicitudProcesada { get; set; }
    }
}
