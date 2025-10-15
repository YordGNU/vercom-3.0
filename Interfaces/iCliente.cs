using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vercom.Interfaces
{
    public class iCliente
    {
        public int ClienteID { get; internal set; }
        public string Nombre { get; internal set; }
        public string Nacionalidad { get; internal set; }
        public string Direccion { get; internal set; }
        public string Provincia { get; internal set; }
        public string Localidad { get; internal set; }
        public string Municipio { get; internal set; }
        public string NIT { get; internal set; }
        public string REEUP { get; internal set; }
        public string RENAE { get; internal set; }
        public int? TipoClienteID { get; internal set; }
        public string Tipo { get; internal set; }

        public List<DTOCuenta> Cuentas;
    }

    public class DTOCuenta
    {
        public int? CuentaID { get; internal set; }
        public string No { get; internal set; }
        public string Tipo { get; internal set; }
        public string Agencia { get; internal set; }
        public string Banco { get; internal set; }
        public string Titular { get; internal set; }
        public string Direccion { get; internal set; }
    }
}