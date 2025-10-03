using System;

namespace vercom.Interfaces
{
    public class iOperacionXFecha
    {
        public DateTime Fecha { get; set; }
        public string FechaTexto => Fecha.ToString("d"); // corto según cultura actual
        public string TipoOperacion { get; set; }
        public double? Cantidad { get; set; }
        public double? Importe { get; set; }
        public double? Costo { get; set; }
        public double? Utilidad { get; set; }
        public double? Porciento { get; set; }
    }
}