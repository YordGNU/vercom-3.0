using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iMayoristaXMpago
    {
        public string MedioPago { get; set; }
        public double ImporteTotal { get; set; }
        public double CantidadTotal { get; set; }
        public double PorcentajeParticipacion { get; set; }
    }
}