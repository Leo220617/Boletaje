using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boletaje.Models
{
    public class ProductosViewModel
    {
        
        public Producto[] Productos { get; set; }
    }
    public class Producto
    {
        public string manufSN { get; set; }
        public string internalSN { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string customer { get; set; }
    }
}
