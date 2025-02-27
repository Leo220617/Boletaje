using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class GarantiasViewModel
    {
        public int id { get; set; }

        [StringLength(50)]
        public string idSAP { get; set; }

        [StringLength(500)]
        public string Nombre { get; set; }
        public bool taller { get; set; }
    }
}
