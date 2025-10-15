using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iProductoServi
    {
        public int ProductoID { get; set; }        
        public string Nombre { get; set; }
        public double? Precio { get; set; }       
        public double? Costo { get; set; }              
    }
}