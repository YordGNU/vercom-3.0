using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iCierreCajaResumen
    {   

        public int CierreID { get; set; }
        public DateTime FechaCierre { get; set; }
        public string FechaTexto => FechaCierre.ToString("g");
        public string Caja { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalEgresos { get; set; }
        public decimal SaldoSubMayores { get; set; }
        public decimal SaldoCajaPrincipal { get; set; }
        public decimal SaldoFinal { get; set; }
        public string Usuario { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }

    }
}