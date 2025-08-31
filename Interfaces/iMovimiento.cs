using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iMovimiento
    {
        public int MovimientoID {  get; set; }
        public DateTime Fecha { get; set; }
        public string FechaTexto => Fecha.ToString("G");
        public string SubMayor { get; set; }
        public int SubMayorID { get; set; }
        public decimal SaldoSubMayor { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal Monto {  get; set; }
        public string Concepto {  get; set; }
        public string ReferenciaExterna {  get; set; }
        public string MetodoPago {  get; set; }
        public string Usuario {  get; set; }
    }
}