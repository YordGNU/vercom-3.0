using System;

namespace vercom.Interfaces
{
    public class iOperacionXfecha
    {
        public string fecha { get; set; }
        public string tipo { get; set; }
        public DateTime fechaLabel { get; set; }
        public double? cantidad { get; set; }
        public decimal porciento { get; set; }
        public double? importe { get; set; }
        public double? costo { get; set; }
        public double? utilidad { get; set; }
    }
}