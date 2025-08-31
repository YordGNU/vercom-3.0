using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iProductoRecientes
    {
        public int ProductoID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public double? Precio { get; set; }       // ← usar double en vez de float
        public double? Costo { get; set; }        // ← usar double en vez de float
        public DateTime FechaCreacion { get; set; }
        public string FechaTexto => FechaCreacion.ToString("d");
        public string CategoriaNombre { get; set; }
        public string UnidadNombre { get; set; }
        public string AreaNombre { get; set; }
        public bool Activo { get; set; }
    }
}