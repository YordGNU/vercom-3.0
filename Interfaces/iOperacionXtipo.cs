namespace vercom.Interfaces
{
    public class iOperacionXtipo
    {
        public string tipo { get; set; }           // tipo de operación
        public double? cantidad { get; set; }
        public double? importe { get; set; }
        public double? costo { get; set; }
        public double? utilidad { get; set; }
        public double? porciento { get; set; }     // se calcula en el controlador
    }
}