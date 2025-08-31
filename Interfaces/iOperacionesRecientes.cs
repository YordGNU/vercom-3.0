using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iOperacionesRecientes
    {
        public int OperacionID { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaTexto => Fecha.ToString("d");
        public string PuntoVenta { get; set; }
        public string Producto { get; set; }
        public string ProductoCod { get; set; }
        public string TipoPago { get; set; }
        public string TipoOperacion { get; set; }
        public float Cantidad { get; set; }
        public float Importe { get; set; }
    }
}