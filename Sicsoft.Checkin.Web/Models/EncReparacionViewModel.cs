﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class EncReparacionViewModel
    {
        public int id { get; set; }
        public int idLlamada { get; set; }
        public int idTecnico { get; set; }
        public int idDiagnostico { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int TipoReparacion { get; set; }
        public string idProductoArreglar { get; set; }
        public int Status { get; set; }
        public bool ProcesadaSAP { get; set; }
        public string Comentarios { get; set; }
        public string BodegaOrigen { get; set; }
        public string BodegaFinal { get; set; }
        public DetReparacionViewModel[] Detalle { get; set; }
    }
}
