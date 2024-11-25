using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class ClientesViewModel
    {
        
        public cliente[] Clientes { get; set; }
    }
    public class cliente
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Phone1 { get; set; }
        public string E_Mail { get; set; }
        public string Activo { get; set; }
        public string Exonerado { get; set; }
        public string Cedula { get; set; }
        public string idCondPago { get; set; }
    }
}
