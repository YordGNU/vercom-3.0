using System;

namespace vercom.Interfaces
{
    public class iOperacion
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string FechaTexto => fecha.ToString("d");
        public string tipo_operacion { get; set; }
        public int tipo_operacion_id { get; set; }
        public float cantidad { get; set; } 
        public string unidad { get; set; }
        public float importe { get; set; }
        public string punto_venta { get; set; }
        public int punto_venta_id { get; set; }
        public string prod_cod { get; set; }
        public int prod_id { get; set; }
        public string prod_nombre { get; set; }
        public string categoria { get; set; }
        public int categoria_id { get; set; }
        public string tipo_pago { get; set; }
        public int? tipo_pago_id { get; set; }
    }
}