using InversionGloblalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sicsoft.CostaRica.Checkin.Web.Models
{
    public class LoginDevolucion
    {
        public int id { get; set; }
        public string idRol { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string Clave { get; set; }
        public string token { get; set; }
        public string CodigoVendedor { get; set; }
        public string Bodega { get; set; }
        public string PIN { get; set; }
        public string idTecnico { get; set; }
        public int idSucursal { get; set; }
        public List<SeguridadRolesModulos> Seguridad { get; set; }
    }
}
