namespace vercom.Interfaces
{
    public class iOperacionXproducto
    {
        public string cod { get; set; }
        public string tipo { get; set; }
        public string producto { get; set; }
        public double? cantidad { get; set; }
        public string unidad { get; set; }
        public double? porciento { get; set; }
        public double? importe { get; set; }
        public double? costo { get; set; }
        public double? utilidad { get; set; }
    }
}