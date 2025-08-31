using System.Data.SqlClient;
using System.Web.Mvc;
using System;

namespace vercom.Interfaces
{
    public class iOperacionXPunto
    {
        public int PuntoVentaID { get; set; }
        public string PuntoVenta { get; set; }
        public string TipoOperacion { get; set; }
        public double? cantidad { get; set; }
        public double? importe { get; set; }
        public double? costo { get; set; }
        public double? utilidad { get; set; }
        public double? porciento { get; set; }
    }

}