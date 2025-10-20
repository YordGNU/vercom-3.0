using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iNegocio
    {
        public DateTime? Fecha { get; internal set; }
        public string FechaTexto => Fecha.Value.ToString("d");
        public string Cliente { get; internal set; }
        public int? ClienteID { get; internal set; }
        public string ProductoServi { get; internal set; }
        public float? Cantidad { get; internal set; }
        public float? Importe { get; internal set; }
        public string FOperacion { get; internal set; }
        public int? FOperacionID { get; internal set; }
        public string TFactura { get; internal set; }
        public string MPago { get; internal set; }
        public string Factura { get; internal set; }
        public int NegocioID { get; internal set; }
        public int? ProductoServiID { get; internal set; }
        public int? TFacturaID { get; internal set; }
        public int? MPagoID { get; internal set; }
    }
}