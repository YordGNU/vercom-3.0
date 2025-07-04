using System.Collections.Generic;

namespace vercom.Interfaces
{
    public class iFlujoResumen
    {
        public int? areaID { get; set; }
        public string tipo { get; set; }
        public string area { get; set; }
        public double? saldo { get; set; }
        public double? saldoFinal { get; set; }
        public float? entrada { get; set; }
        public float? salida { get; set; }
        public List<float?> listentrada { get; set; }
        public List<float?> listsalida { get; set; }
        public List<string> conceptoEntrada { get; set; }
        public List<string> conceptoSalida { get; set; }


    }

}