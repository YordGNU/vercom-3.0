namespace vercom.Interfaces
{
    public class iOperacion
    {
        public int id { get; set; }
        public string fecha { get; set; }
        public string tipo_operacion { get; set; }
        public double? cantidad { get; set; }

        public string unidad { get; set; }

        public double? importe { get; set; }
        public string punto_venta { get; set; }
        public string prod_cod { get; set; }
        public string prod_nombre { get; set; }
    }
}