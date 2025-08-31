using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iSpIpvBaseResult
    {     
        public int productoid { get; set; }
        public string cod { get; set; }
        public string producto { get; set; }
        public int unidadid { get; set; }
        public double precio { get; set; }
        public double cantidad_saldo { get; set; }
        public double importe_saldo { get; set; }
        public double cantidad_entrada { get; set; }
        public double cantidad_venta { get; set; }
        public double cantidad_devolucion { get; set; }
        public double cantidad_merma { get; set; }

    }
}