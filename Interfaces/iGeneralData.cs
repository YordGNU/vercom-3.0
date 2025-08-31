using System.Collections.Generic;
namespace vercom.Interfaces
{
    public class iGeneralData
    {
        public double? importe_venta { get; set; }
        public double? costo_venta { get; set; }
        public double? utilidades { get; set; }
        public double? pcien_importe { get; set; }
        public double? pcien_costo { get; set; }
        public double? pcien_utilidad { get; set; }
        public List<iTipoPago> tipo_pago { get; set; }
    }

    public class iTipoPago
    {
        public string tipo { get; set; }
        public double? cantidad { get; set; }
        public double? porciento { get; set; }
    }

    public class iGeneralSPResult
    {
        public double? ImporteVentas { get; set; }
        public double? CostoVentas { get; set; }
        public int CantidadVentas { get; set; }
        public int VentasEfectivo { get; set; }
        public int VentasTransferencia { get; set; }
    }

}