using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iSubMayoresResumen
    {
        public int SubMayorID { get; set; }
        public string SubMayorNombre { get; set; }
        public decimal TotalEntrada { get; set; }
        public decimal TotalSalida { get; set; }
        public decimal SaldoActual { get; set; }
    }
}