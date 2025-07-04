using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iProductoOperacion
    {
        public int ProductoID { get; set; }
        public decimal? CantidadEnPuntoVenta { get; set; } // Convertido a decimal
        public decimal? Cantidad { get; set; } // Convertido a decimal
        public decimal? CantidadRestante { get; set; } // Convertido a decimal
        public decimal? PrecioUnitario { get; set; } // Convertido a decimal
        public decimal? Importe { get; set; } // Convertido a decimal
        public int? TipoPagoID { get; set; } // Hacerlo nullable si no siempre tiene valor
    }
}