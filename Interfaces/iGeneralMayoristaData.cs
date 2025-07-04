using System.Collections.Generic;

namespace vercom.Interfaces
{
    public class iGeneralMayoristaData
    {
        public float? TotalImpVentas { get; internal set; }
        public float? TotalCostVentas { get; internal set; }
        public float? TotalUtilidades { get; internal set; }
        public string modaClienteNombre { get; internal set; }
        public string modaProductoServi { get; internal set; }
        public string modaFormaOperacion { get; internal set; }
       
    }  
}