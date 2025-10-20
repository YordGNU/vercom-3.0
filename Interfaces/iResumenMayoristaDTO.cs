using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iResumenMayoristaDTO
    {
        public double ImporteVenta { get; set; }
        public double CostoTotal { get; set; }
        public double Utilidad { get; set; }
        public string ClienteFrecuente { get; set; }
        public string ProductoPopular { get; set; }
        public string CanalPreferido { get; set; }

    }
}