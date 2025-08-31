namespace vercom.Interfaces
{
    public class iOperacionXproducto
    {
        public int ProductoID { get; set; }
        public string Cod { get; set; }
        public string Producto { get; set; }
        public string Unidad { get; set; }
        public string TipoOperacion { get; set; }
        public double? cantidad { get; set; }
        public double? importe { get; set; }
        public double? costo { get; set; }
        public double? utilidad { get; set; }
        public double? porciento { get; set; }
    }
}