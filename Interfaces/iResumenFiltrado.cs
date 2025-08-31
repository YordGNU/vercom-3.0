using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iResumenFiltrado
    {
        public int CantidadMovimientos { get; set; }
        public decimal TotalEntradas { get; set; }
        public decimal TotalSalidas { get; set; }
        public decimal? SaldoActual { get; set; } // si es sub-mayor
        public decimal? SaldoSubMayores { get; set; }
        public decimal? SaldoCajaPrincipal { get; set; }
        public decimal? SaldoTotal { get; set; }

    }
}