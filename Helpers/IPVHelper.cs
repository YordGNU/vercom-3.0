using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using vercom.Models;
using vercom.Interfaces;

namespace vercom.Helpers
{
    public static class IPVHelper
    {

        static VERCOMEntities db = new VERCOMEntities();

        public static List<iPVAnalitic> GenerarAnaliticoDesdeSP(DateTime inicio, DateTime fin, int pventa, int categ)
        {
            var resultado = new List<iPVAnalitic>();
     

            // Diccionario de unidades para visualización
            var unidades = db.unidad.Select(u => new { u.id, u.unidad1 }).ToDictionary(x => x.id, x => x.unidad1);

            // Obtener nombre del punto de venta
            string puntoNombre = "";
            if (pventa != 0){ puntoNombre = db.punto_venta.Where(p => p.id == pventa).Select(p => p.nombre).FirstOrDefault();}

            // Parámetros para el SP
            var paramList = new[]
            {
            new SqlParameter("@inicio", SqlDbType.Date) { Value = inicio },
            new SqlParameter("@fin",    SqlDbType.Date) { Value = fin },
            new SqlParameter("@pventa", SqlDbType.Int)  { Value = pventa },
            new SqlParameter("@categ",  SqlDbType.Int)  { Value = categ }
            };

            db.Database.CommandTimeout = 120;
            var spData = db.Database.SqlQuery<iSpIpvBaseResult>(
                "EXEC sp_ObtenerDatosIPV @inicio, @fin, @pventa, @categ",
                paramList
            ).ToList();

            foreach (var r in spData)
            {
                double impEntrada = r.cantidad_entrada * r.precio;
                double impVenta = r.cantidad_venta * r.precio;
                double impDevolucion = r.cantidad_devolucion * r.precio;
                double impMerma = r.cantidad_merma * r.precio;
                double saldoFinal = r.cantidad_saldo + r.cantidad_entrada - (r.cantidad_venta + r.cantidad_devolucion + r.cantidad_merma);

                resultado.Add(new iPVAnalitic
                {      
                    id = r.productoid,
                    cod = r.cod,
                    producto = r.producto,
                    unidad = unidades.TryGetValue(r.unidadid, out var u) ? u : null,
                    precio_venta = r.precio,
                    cantidad_saldo = r.cantidad_saldo,
                    importe_saldo = r.importe_saldo,
                    cantidad_entrada = r.cantidad_entrada,
                    importe_entrada = impEntrada,
                    cantidad_venta = r.cantidad_venta,
                    importe_venta = impVenta,
                    cantidad_devolucion = r.cantidad_devolucion,
                    importe_devolucion = impDevolucion,
                    cantidad_merma = r.cantidad_merma,
                    importe_merma = impMerma,
                    final_saldo = saldoFinal,
                    final_importe = saldoFinal * r.precio,
                    punto_nombre = puntoNombre,
                    fecha = fin
                });
            }

            return resultado;
        }
    }
}