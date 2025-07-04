using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iProductosXpunto
    {
      

        public string nombre { get; internal set; }
        public int id { get; internal set; }
        public string cod { get; internal set; }
        public float? cant_saldo { get; internal set; }
        public object precio { get; internal set; }
    }
}