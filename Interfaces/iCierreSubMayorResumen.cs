using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iCierreSubMayorResumen
    {
        public int? Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string FechaTexto => FechaRegistro.ToString("g");
        public int? CierreID { get; set; }
        public int? SubMayorID { get; set; }
        public string SubMayorNombre { get; set; }
        public decimal? SaldoFinal { get; set; }
        public List<MovimientoDTO> Entradas { get; set; }
        public decimal TotalEntradas => Entradas.Sum(e => e.Monto);
        public List<MovimientoDTO> Salidas { get; set; }
        public decimal TotalSalidas => Salidas.Sum(e => e.Monto);
    }


    public class MovimientoDTO
    {
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; }
        public string FechaTexto => Fecha.ToString("g");
        public decimal Monto { get; set; }
        public string Concepto { get; set; }
        public string Referencia { get; set; }
    }

}