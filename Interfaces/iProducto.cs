using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iProducto
    {
        public int ProductoId { get; internal set; }
        public string Nombre { get; internal set; }
        public string Cod { get; internal set; }
        public double? Precio { get; internal set; }
        public double? Costo { get; internal set; }
        public int? UnidadID { get; internal set; }
        public int? CategoriaID { get; internal set; }
        public int? AreaID { get; internal set; }
        public bool? Activo { get; internal set; }
     
    }
}