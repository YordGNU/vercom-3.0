using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using vercom.Interfaces;
using vercom.Models;
namespace vercom.Controllers
{

    [Authorize]
    public class AnalisisController : Controller
    {
        VERCOMEntities db = new VERCOMEntities();

        [RBAC]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incremento()
        {
            return View();
        }

        public ContentResult _productosXpuntoFTdate(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            List<producto> iData = new List<producto>();
            var cvDateINI = Convert.ToDateTime(inicio);
            var cvDateFIN = Convert.ToDateTime(fin);
            var ftdata = (from r in db.operacion where (r.fecha >= cvDateINI & r.fecha <= cvDateFIN) select r);

            if (toper != 0) { ftdata = ftdata.Where(x => x.tipo_operacionid == toper); }
            if (pventa != 0) { ftdata = ftdata.Where(x => x.punto_ventaid == pventa); }
            if (categ != 0) { ftdata = ftdata.Where(x => x.producto.categoriaid == categ); }
            if (producto != 0) { ftdata = ftdata.Where(x => x.productoid == producto); }
            if (tpago != 0) { ftdata = ftdata.Where(x => x.tipo_pagoid == tpago); }
            if (area != 0) { ftdata = ftdata.Where(x => x.producto.areaid == area); }

            var ftgroup = (from r in ftdata group r by r.producto.id into rp select new { grp = rp.Key, cod = rp.Select(x => x.producto.cod).FirstOrDefault(), nombre = rp.Select(x => x.producto.nombre).FirstOrDefault() }).OrderBy(x => x.grp).ToList();

            foreach (var item in ftgroup)
            {
                producto iVal = new producto
                {
                    id = item.grp,
                    cod = item.cod,
                    nombre = item.nombre,
                };
                iData.Add(iVal);
            }
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }
        [HttpGet]
        public ContentResult ChartSxTipoOperacionFTdate(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            List<iOperacionXtipo> iData = new List<iOperacionXtipo>();
            var cvDateINI = Convert.ToDateTime(inicio);
            var cvDateFIN = Convert.ToDateTime(fin);

            var ftdata = (from d in db.operacion where (d.fecha >= cvDateINI & d.fecha <= cvDateFIN) select d);

            if (toper != 0) { ftdata = ftdata.Where(x => x.tipo_operacionid == toper); }
            if (pventa != 0) { ftdata = ftdata.Where(x => x.punto_ventaid == pventa); }
            if (categ != 0) { ftdata = ftdata.Where(x => x.producto.categoriaid == categ); }
            if (producto != 0) { ftdata = ftdata.Where(x => x.productoid == producto); }
            if (tpago != 0) { ftdata = ftdata.Where(x => x.tipo_pagoid == tpago); }
            if (area != 0) { ftdata = ftdata.Where(x => x.producto.areaid == area); }

            double total_cant = ftdata.Count();
            double total_importe = 0;
            decimal porcent = 0;

            if (total_cant > 0) total_importe = (double)ftdata.Sum(i => i.cantidad * i.producto.precio);

            var ftgroup = (from r in ftdata group r by r.tipo_operacion.tipo into rp select new { grp = rp.Key, Count = rp.Select(x => x.cantidad).Sum(), Importe = rp.Select(x => x.cantidad * x.producto.precio).Sum() }).OrderBy(x => x.grp).ToList();

            // Obtener el título dinámico
            string puntoVentaNombre = db.punto_venta.Where(p => p.id == pventa).Select(p => p.nombre).FirstOrDefault() ?? "";
            string categoriaNombre = db.categoria.Where(c => c.id == categ).Select(c => c.clave).FirstOrDefault() ?? "";
            string tipoPago = db.tipo_pago.Where(c => c.id == tpago).Select(c => c.tipo).FirstOrDefault() ?? "";
            string titulo = $"PUNTO: {puntoVentaNombre}, FECHA: {cvDateINI.ToString("d")} AL {cvDateFIN.ToString("d")}, CATEGORIA: {categoriaNombre},  T.PAGO: {tipoPago}";

            foreach (var item in ftgroup)
            {
                porcent = (decimal)((item.Importe / total_importe) * 100);
                porcent = Convert.ToDecimal(String.Format($"{porcent:F3}"));

                iOperacionXtipo iVal = new iOperacionXtipo
                {
                    tipo = item.grp,
                    cantidad = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Count)),
                    porciento = porcent,
                    importe = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Importe)),
                };
                iData.Add(iVal);
            }

            return Content(JsonConvert.SerializeObject(new { iData, titulo }), "application/json");
        }
        [HttpGet]
        public ContentResult ChartSxiOperacionXpuntoFTdate(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            List<iOperacionXpunto> iData = new List<iOperacionXpunto>();
            var cvDateINI = Convert.ToDateTime(inicio);
            var cvDateFIN = Convert.ToDateTime(fin);
            var ftdata = (from d in db.operacion where (d.fecha >= cvDateINI & d.fecha <= cvDateFIN) select d);

            if (toper != 0) { ftdata = ftdata.Where(x => x.tipo_operacionid == toper); }
            if (pventa != 0) { ftdata = ftdata.Where(x => x.punto_ventaid == pventa); }
            if (categ != 0) { ftdata = ftdata.Where(x => x.producto.categoriaid == categ); }
            if (producto != 0) { ftdata = ftdata.Where(x => x.productoid == producto); }
            if (tpago != 0) { ftdata = ftdata.Where(x => x.tipo_pagoid == tpago); }
            if (area != 0) { ftdata = ftdata.Where(x => x.producto.areaid == area); }

            double total_cant = ftdata.Count();
            double total_importe = 0;
            decimal porcent = 0;

            if (total_cant > 0) total_importe = (double)ftdata.Sum(i => i.cantidad * i.producto.precio);

            var ftgroup = ftdata
                .GroupBy(r => new { r.punto_venta.nombre, r.tipo_operacion.tipo })
                .Select(rp => new
                {
                    Punto = rp.Key.nombre,
                    TipoOperacion = rp.Key.tipo,
                    Count = rp.Sum(x => x.cantidad),
                    Importe = rp.Sum(x => x.cantidad * x.producto.precio),
                    Costo = rp.Sum(x => x.cantidad * x.producto.costo)
                })
                .OrderBy(x => x.Punto)
                .ThenBy(x => x.TipoOperacion)
                .ToList();

            // Obtener el título dinámico
            string puntoVentaNombre = db.punto_venta.Where(p => p.id == pventa).Select(p => p.nombre).FirstOrDefault() ?? "";
            string categoriaNombre = db.categoria.Where(c => c.id == categ).Select(c => c.clave).FirstOrDefault() ?? "";
            string tipoPago = db.tipo_pago.Where(c => c.id == tpago).Select(c => c.tipo).FirstOrDefault() ?? "";
            string titulo = $"PUNTO: {puntoVentaNombre}, FECHA: {cvDateINI.ToString("d")} AL {cvDateFIN.ToString("d")}, CATEGORIA: {categoriaNombre},  T.PAGO: {tipoPago}";

            foreach (var item in ftgroup)
            {
                porcent = (decimal)((item.Importe / total_importe) * 100);
                porcent = Convert.ToDecimal(String.Format($"{porcent:F2}"));
                double? utilidad = item.Importe - item.Costo;

                iOperacionXpunto iVal = new iOperacionXpunto
                {
                    punto = item.Punto,
                    tipo = item.TipoOperacion,
                    cantidad = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Count)),
                    porciento = porcent,
                    importe = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Importe)),
                    costo = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Costo)),
                    utilidad = Convert.ToDouble(String.Format("{0:#,##0.00}", utilidad)),

                };
                iData.Add(iVal);
            }
            return Content(JsonConvert.SerializeObject(new { iData, titulo }), "application/json");
        }

        [HttpGet]
        public ContentResult ChartSxiOperacionXproductoFTdate(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            List<iOperacionXproducto> iData = new List<iOperacionXproducto>();
            var cvDateINI = Convert.ToDateTime(inicio);
            var cvDateFIN = Convert.ToDateTime(fin);
            var ftdata = (from d in db.operacion where (d.fecha >= cvDateINI & d.fecha <= cvDateFIN) select d);

            if (toper != 0) { ftdata = ftdata.Where(x => x.tipo_operacionid == toper); }
            if (pventa != 0) { ftdata = ftdata.Where(x => x.punto_ventaid == pventa); }
            if (categ != 0) { ftdata = ftdata.Where(x => x.producto.categoriaid == categ); }
            if (producto != 0) { ftdata = ftdata.Where(x => x.productoid == producto); }
            if (tpago != 0) { ftdata = ftdata.Where(x => x.tipo_pagoid == tpago); }
            if (area != 0) { ftdata = ftdata.Where(x => x.producto.areaid == area); }

            double? total_cant = ftdata.Count();

            double? total_importe = 0;
            double? porcent = 0;


            if (total_cant > 0)
            {
                total_importe = ftdata.Sum(i => i.cantidad * i.producto.precio);
            }

            var ftgroup = ftdata
                .GroupBy(r => new { r.producto.id, r.tipo_operacion.tipo })
                .Select(rp => new
                {
                    Cod = rp.Select(x => x.producto.cod).FirstOrDefault(),
                    Producto = rp.Select(x => x.producto.nombre).FirstOrDefault(),
                    Unidad = rp.Select(x => x.producto.unidad.unidad1).FirstOrDefault(),
                    Count = rp.Count(),
                    TipoOperacion = rp.Key.tipo,
                    Cant = rp.Sum(x => x.cantidad),
                    Importe = rp.Sum(x => x.cantidad * x.producto.precio),
                    Costo = rp.Sum(x => x.cantidad * x.producto.costo)
                })
                .OrderBy(x => x.Producto)
                .ThenBy(x => x.TipoOperacion)
                .ToList();

            // Obtener el título dinámico            
            string puntoVentaNombre = db.punto_venta.Where(p => p.id == pventa).Select(p => p.nombre).FirstOrDefault() ?? "";
            string categoriaNombre = db.categoria.Where(c => c.id == categ).Select(c => c.clave).FirstOrDefault() ?? "";
            string tipoPago = db.tipo_pago.Where(c => c.id == tpago).Select(c => c.tipo).FirstOrDefault() ?? "";
            string titulo = $"PUNTO: {puntoVentaNombre}, FECHA: {cvDateINI.ToString("d")} AL {cvDateFIN.ToString("d")}, CATEGORIA: {categoriaNombre},  T.PAGO: {tipoPago}";           

            foreach (var item in ftgroup)
            {
                porcent = (item.Importe / total_importe) * 100;  
                double? utilidad = item.Importe - item.Costo;

                iOperacionXproducto iVal = new iOperacionXproducto
                {
                    cod = item.Cod,
                    producto = item.Producto,
                    tipo = item.TipoOperacion,
                    cantidad = item.Cant,
                    unidad = item.Unidad,
                    porciento = porcent,
                    importe =item.Importe,
                    costo =  item.Costo,
                    utilidad = utilidad
                };
                iData.Add(iVal);
            }
            return Content(JsonConvert.SerializeObject(new { iData, titulo }), "application/json");
        }

        [HttpGet]
        public ContentResult ChartSxiOperacionXFecha(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            List<iOperacionXfecha> iData = new List<iOperacionXfecha>();
            var cvDateINI = Convert.ToDateTime(inicio);
            var cvDateFIN = Convert.ToDateTime(fin);
            var ftdata = (from d in db.operacion where (d.fecha >= cvDateINI & d.fecha <= cvDateFIN) select d);

            if (toper != 0) { ftdata = ftdata.Where(x => x.tipo_operacionid == toper); }
            if (pventa != 0) { ftdata = ftdata.Where(x => x.punto_ventaid == pventa); }
            if (categ != 0) { ftdata = ftdata.Where(x => x.producto.categoriaid == categ); }
            if (producto != 0) { ftdata = ftdata.Where(x => x.productoid == producto); }
            if (tpago != 0) { ftdata = ftdata.Where(x => x.tipo_pagoid == tpago); }
            if (area != 0) { ftdata = ftdata.Where(x => x.producto.areaid == area); }

            double total_cant = ftdata.Count();
            double total_importe = 0;
            decimal porcent = 0;

            if (total_cant > 0) total_importe = (double)ftdata.Sum(i => i.cantidad * i.producto.precio);

            var ftgroup = ftdata
                .GroupBy(r => new { r.fecha, r.tipo_operacion.tipo })
                .Select(rp => new
                {
                    Fecha = rp.Key.fecha,
                    TipoOperacion = rp.Key.tipo,
                    Count = rp.Sum(x => x.cantidad),
                    Importe = rp.Sum(x => x.cantidad * x.producto.precio),
                    Costo = rp.Sum(x => x.cantidad * x.producto.precio)
                })
                .OrderBy(x => x.Fecha)
                .ThenBy(x => x.TipoOperacion)
                .ToList();

            // Obtener el título dinámico
            string puntoVentaNombre = db.punto_venta.Where(p => p.id == pventa).Select(p => p.nombre).FirstOrDefault() ?? "";
            string categoriaNombre = db.categoria.Where(c => c.id == categ).Select(c => c.clave).FirstOrDefault() ?? "";
            string tipoPago = db.tipo_pago.Where(c => c.id == tpago).Select(c => c.tipo).FirstOrDefault() ?? "";
            string titulo = $"PUNTO: {puntoVentaNombre}, FECHA: {cvDateINI.ToString("d")} AL {cvDateFIN.ToString("d")}, CATEGORIA: {categoriaNombre},  T.PAGO: {tipoPago}";

            foreach (var item in ftgroup)
            {

                porcent = (decimal)((item.Importe / total_importe) * 100);
                porcent = Convert.ToDecimal(String.Format($"{porcent:F2}"));
                double? utilidad = (item.Importe - item.Costo);

                iOperacionXfecha iVal = new iOperacionXfecha
                {
                    fecha = item.Fecha.Value.ToString("d"),
                    tipo = item.TipoOperacion,
                    fechaLabel = (DateTime)item.Fecha,
                    cantidad = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Count)),
                    porciento = porcent,
                    importe = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Importe)),
                    costo = Convert.ToDouble(String.Format("{0:#,##0.00}", item.Costo)),
                    utilidad = Convert.ToDouble(String.Format("{0:#,##0.00}", utilidad)),
                };
                iData.Add(iVal);
            }

            return Content(JsonConvert.SerializeObject(new { iData, titulo }), "application/json");
        }
        [HttpGet]
        public ContentResult ChartSxiGeneralData(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            List<iGeneralData> iData = new List<iGeneralData>();

            DateTime cvDateINI = Convert.ToDateTime(inicio);
            DateTime cvDateFIN = Convert.ToDateTime(fin);

            var ftdata = db.operacion.Where(d => d.fecha >= cvDateINI && d.fecha <= cvDateFIN);

            if (toper != 0) ftdata = ftdata.Where(x => x.tipo_operacionid == toper);
            if (pventa != 0) ftdata = ftdata.Where(x => x.punto_ventaid == pventa);
            if (categ != 0) ftdata = ftdata.Where(x => x.producto.categoriaid == categ);
            if (producto != 0) ftdata = ftdata.Where(x => x.productoid == producto);
            if (tpago != 0) ftdata = ftdata.Where(x => x.tipo_pagoid == tpago);
            if (area != 0) ftdata = ftdata.Where(x => x.producto.areaid == area);

            var dataList = ftdata.ToList();

            var TotalImpVentas = dataList.Where(i => i.tipo_operacionid == 2).Sum(i => i.cantidad * i.producto.precio);
            var TotalCostVentas = dataList.Where(i => i.tipo_operacionid == 2).Sum(i => i.cantidad * i.producto.costo);
            var TotalUtilidades = TotalImpVentas - TotalCostVentas;
            var total_cantventas = dataList.Count;

            double impventas = (double)TotalImpVentas;
            double costventas = (double)TotalCostVentas;
            double? utilidad = impventas - costventas;

            decimal tVefectivo = dataList.Count(sx => sx.tipo_pagoid == 1);
            decimal tVtransfer = dataList.Count(sx => sx.tipo_pagoid == 2);

            decimal porciento_efe = 0;
            decimal porciento_tra = 0;
            decimal porciento_imp = 0;
            decimal porciento_cost = 0;
            decimal porciento_util = 0;

            if (TotalImpVentas != 0 && total_cantventas != 0)
            {
                porciento_efe = Math.Round((tVefectivo / total_cantventas) * 100, 2);
                porciento_tra = Math.Round((tVtransfer / total_cantventas) * 100, 2);
                porciento_imp = (decimal)Math.Round((impventas / (double)TotalImpVentas) * 100, 2);
                porciento_cost = (decimal)Math.Round((costventas / (double)TotalImpVentas) * 100, 2);
                porciento_util = (decimal)Math.Round((double)((utilidad / (double)TotalImpVentas) * 100), 2);
            }

            List<iTipoPago> iTipos = new List<iTipoPago>
            {
                new iTipoPago { tipo = "Efectivo", cantidad = (double?)tVefectivo, porciento = (double?)porciento_efe },
                new iTipoPago { tipo = "Transferencia", cantidad = (double?)tVtransfer, porciento = (double?)porciento_tra }
            };

            iData.Add(new iGeneralData
            {
                importe_venta = impventas,
                costo_venta = costventas,
                utilidades = utilidad,
                pcien_importe = (double?)porciento_imp,
                pcien_costo = (double?)porciento_cost,
                pcien_utilidad = (double?)porciento_util,
                tipo_pago = iTipos
            });

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }


        public ContentResult incremetoProduccion(string inicio, string fin, int producto = 0, double gastoOperativo = 0, int numTraba = 0, int prodProTrab = 0, decimal meta = 0)
        {
            List<iIncremento> iData = new List<iIncremento>();
            var cvDateINI = Convert.ToDateTime(inicio);
            var cvDateFIN = Convert.ToDateTime(fin);

            var prod = (from r in db.producto where r.id == producto select r);
            var costo = (from r in prod select r.costo).FirstOrDefault();
            var precio = (from r in prod select r.precio).FirstOrDefault();
            var unidad = (from r in prod select r.unidad.unidad1).FirstOrDefault();

            var ftdata = (from d in db.operacion where (d.fecha >= cvDateINI & d.fecha <= cvDateFIN) & (d.tipo_operacionid == 2) select d);
            var ventActual = (from r in ftdata where (r.productoid == producto) select r.cantidad).Sum();

            var ingresoActual = ventActual * precio;
            var costoActual = ventActual * costo;

            var utilidadActual = ingresoActual - costoActual;


            var incrementoVenta = Convert.ToInt16((float?)(meta / 100) * ventActual);

            var incremento = incrementoVenta;

            incrementoVenta = (short)(incremento + ventActual);


            var incrementoProdTrab = (incrementoVenta / prodProTrab);
            incrementoProdTrab = (incrementoProdTrab) - (numTraba);

            var costoTotal = (costo * (double?)incrementoVenta);
            var ingresoTotal = (precio * (double?)incrementoVenta);

            var beneficioBruto = (ingresoTotal - costoTotal);
            var beneficio = (ingresoTotal - costoTotal) - gastoOperativo;

            iIncremento iVal = new iIncremento
            {
                ventaActual = ventActual,
                unidad = unidad,
                incremento = Convert.ToDouble(String.Format("{0:#,##0.00}", incremento)),
                aumentotrab = (int)incrementoProdTrab,
                ingresoActual = Convert.ToDouble(String.Format("{0:#,##0.00}", ingresoActual)),
                ingreso = Convert.ToDouble(String.Format("{0:#,##0.00}", ingresoTotal)),
                costo = Convert.ToDouble(String.Format("{0:#,##0.00}", costoTotal)),
                costoActual = Convert.ToDouble(String.Format("{0:#,##0.00}", costoActual)),
                utilidadBruto = Convert.ToDouble(String.Format("{0:#,##0.00}", beneficioBruto)),
                utilidadActual = Convert.ToDouble(String.Format("{0:#,##0.00}", utilidadActual)),
                utilidadFinal = Convert.ToDouble(String.Format("{0:#,##0.00}", beneficio)),
            };

            iData.Add(iVal);
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

    }
}
