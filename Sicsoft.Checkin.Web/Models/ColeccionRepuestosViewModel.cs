﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class ColeccionRepuestosViewModel
    {
        public EncReparacionViewModel EncReparacion { get; set; }
        public DetReparacionViewModel[] DetReparacion { get; set; }
        public AdjuntosViewModel[] Adjuntos { get; set; }
    }
}
