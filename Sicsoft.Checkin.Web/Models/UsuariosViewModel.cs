﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InversionGloblalWeb.Models
{
    public class UsuariosViewModel
    {
        public int id { get; set; }

        public int? idRol { get; set; }
        public int idSucursal { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; }

        public bool? Activo { get; set; }

        [StringLength(500)]
        public string Clave { get; set; }
        [StringLength(10)]
        public string CardCode { get; set; }
        public string Bodega { get; set; }
        public string CorreoVentas { get; set; }
        public string Telefono { get; set; }

        public int NumeroDimension { get; set; }

        public string NormaReparto { get; set; }
        public int EmpleadoSAP { get; set; }
        public string  PIN { get; set; }
    }
}
