namespace vercom.Interfaces
{
    public class iOperacionXpunto
    {
        public string punto { get; set; }
        public string tipo { get; set; }

        public double? cantidad { get; set; }
        public double? importe { get; set; }
        public decimal porciento { get; set; }
        public double? costo { get; set; }
        public double? utilidad { get; set; }
    }
}