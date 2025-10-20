using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI;
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
            List<iProductosXpunto> iData = new List<iProductosXpunto>();
            var cvDateINI = Convert.ToDateTime(inicio);
            var cvDateFIN = Convert.ToDateTime(fin);

            var resultado = db.Database.SqlQuery<iProductosXpunto>(
                "EXEC productosXpunto @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                cvDateINI, cvDateFIN, toper, pventa, categ, producto, tpago, area
            );
            iData = resultado.ToList();
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        [HttpGet]
        public JsonResult iGeneralData(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            DateTime.TryParse(inicio, out DateTime cvDateINI);
            DateTime.TryParse(fin, out DateTime cvDateFIN);
            db.Database.CommandTimeout = 120;
            var resultado = db.Database.SqlQuery<iGeneralSPResult>(
                "EXEC sp_ObtenerDatosGenerales @FechaInicio, @FechaFin, @TipoOperacionID, @PuntoVentaID, @CategoriaID, @ProductoID, @TipoPagoID, @AreaID",
                new SqlParameter("@FechaInicio", cvDateINI),
                new SqlParameter("@FechaFin", cvDateFIN),
                new SqlParameter("@TipoOperacionID", toper),
                new SqlParameter("@PuntoVentaID", pventa),
                new SqlParameter("@CategoriaID", categ),
                new SqlParameter("@ProductoID", producto),
                new SqlParameter("@TipoPagoID", tpago),
                new SqlParameter("@AreaID", area)
            ).FirstOrDefault();

            var imp = resultado?.ImporteVentas ?? 0;
            var cost = resultado?.CostoVentas ?? 0;
            var util = imp - cost;
            var total = resultado?.CantidadVentas ?? 0;

            decimal tEfe = resultado?.VentasEfectivo ?? 0;
            decimal tTra = resultado?.VentasTransferencia ?? 0;

            var porcEfe = (total > 0) ? (tEfe / total) * 100 : 0;
            var porcTra = (total > 0) ? (tTra / total) * 100 : 0;
            var porcImp = (imp > 0) ? 100 : 0;
            var porcCost = (imp > 0) ? Math.Round((cost / imp) * 100, 2) : 0;
            var porcUtil = (imp > 0) ? Math.Round((util / imp) * 100, 2) : 0;

            var iTipos = new List<iTipoPago>
            {
                new iTipoPago { tipo = "Efectivo", cantidad = (double?) tEfe, porciento = (double?) porcEfe },
                new iTipoPago { tipo = "Transferencia", cantidad = (double?) tTra, porciento = (double?) porcTra }
            };

            var iData = new iGeneralData
            {
                importe_venta = imp,
                costo_venta = cost,
                utilidades = util,
                pcien_importe = porcImp,
                pcien_costo = porcCost,
                pcien_utilidad = porcUtil,
                tipo_pago = iTipos
            };

            return Json(new { iData }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult iResumenMayorista(string inicio, string fin, int ProductoID = 0, int ClienteID = 0, int OperacionID = 0, int FacturaID = 0, int MedioPagoID = 0, string NoFactura = null)
        {

            DateTime.TryParse(inicio, out DateTime cvDateINI);
            DateTime.TryParse(fin, out DateTime cvDateFIN);
            db.Database.CommandTimeout = 120;
            var resultado = db.Database.SqlQuery<iResumenMayoristaDTO>(
                "EXEC sp_ResumenMayorista @FechaInicio, @FechaFin, @ProductoID, @ClienteID, @OperacionID, @FacturaID, @MedioPagoID, @NoFactura",
                new SqlParameter("@FechaInicio", cvDateINI),
                new SqlParameter("@FechaFin", cvDateFIN),
                new SqlParameter("@ProductoID", ProductoID),
                new SqlParameter("@ClienteID", ClienteID),
                new SqlParameter("@OperacionID", OperacionID),
                new SqlParameter("@FacturaID", FacturaID),
                new SqlParameter("@MedioPagoID", MedioPagoID),
                new SqlParameter("@NoFactura", NoFactura)              
            ).FirstOrDefault();
                 
            return Json(resultado, JsonRequestBehavior.AllowGet);            
        }

        [HttpGet]    
        public JsonResult GetProductosRecientes(int cantidad = 20)
        {
            try
            {
                var productos = db.Database.SqlQuery<iProductoRecientes>(
                    "EXEC sp_ObtenerProductosRecientes @Cantidad",
                    new SqlParameter("@Cantidad", cantidad)
                ).ToList();
                return Json(productos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // 🛠️ Loguear el error si tienes sistema de logs
                return Json(new { error = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetOperacionesRecientes(int cantidad = 20)
        {
            try
            {
                var operaciones = db.Database.SqlQuery<iOperacionesRecientes>(
                    "EXEC sp_ObtenerOperacionesRecientes @Cantidad",
                    new SqlParameter("@Cantidad", cantidad)
                ).ToList();
                return Json(operaciones, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // 🛠️ Loguear el error si tienes sistema de logs
                return Json(new { error = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ChartDxTOperacion(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            DateTime.TryParse(inicio, out DateTime cvDateINI);
            DateTime.TryParse(fin, out DateTime cvDateFIN);
            db.Database.CommandTimeout = 120;
            var datos = db.Database.SqlQuery<iOperacionXtipo>(
                "EXEC sp_ObtenerDistribucionPorOperacion @FechaInicio, @FechaFin, @TipoOperacionID, @PuntoVentaID, @CategoriaID, @ProductoID, @TipoPagoID, @AreaID",
                new SqlParameter("@FechaInicio", cvDateINI),
                new SqlParameter("@FechaFin", cvDateFIN),
                new SqlParameter("@TipoOperacionID", toper),
                new SqlParameter("@PuntoVentaID", pventa),
                new SqlParameter("@CategoriaID", categ),
                new SqlParameter("@ProductoID", producto),
                new SqlParameter("@TipoPagoID", tpago),
                new SqlParameter("@AreaID", area)
            ).ToList();

            double? totalImporte = datos.Sum(x => x.importe);
            foreach (var item in datos)
            {
                item.porciento = (totalImporte > 0) ? (item.importe / totalImporte) * 100 : 0;
            }

            return Json(new { iData = datos }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChartDxFecha(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            DateTime.TryParse(inicio, out DateTime cvDateINI);
            DateTime.TryParse(fin, out DateTime cvDateFIN);
            db.Database.CommandTimeout = 120;
            var datos = db.Database.SqlQuery<iOperacionXFecha>(
                "EXEC sp_ObtenerDistribucionPorFecha @FechaInicio, @FechaFin, @TipoOperacionID, @PuntoVentaID, @CategoriaID, @ProductoID, @TipoPagoID, @AreaID",
                new SqlParameter("@FechaInicio", cvDateINI),
                new SqlParameter("@FechaFin", cvDateFIN),
                new SqlParameter("@TipoOperacionID", toper),
                new SqlParameter("@PuntoVentaID", pventa),
                new SqlParameter("@CategoriaID", categ),
                new SqlParameter("@ProductoID", producto),
                new SqlParameter("@TipoPagoID", tpago),
                new SqlParameter("@AreaID", area)
            ).ToList();

            double? totalImporte = datos.Sum(x => x.Importe);
            foreach (var item in datos)
            {
                item.Porciento = (totalImporte > 0) ? (item.Importe / totalImporte) * 100 : 0;              
            }

            string puntoVentaNombre = db.punto_venta.Where(p => p.id == pventa).Select(p => p.nombre).FirstOrDefault() ?? "";
            string categoriaNombre = db.categoria.Where(c => c.id == categ).Select(c => c.clave).FirstOrDefault() ?? "";
            string tipoPago = db.tipo_pago.Where(p => p.id == tpago).Select(p => p.tipo).FirstOrDefault() ?? "";

            string titulo = $"PUNTO: {puntoVentaNombre}, FECHA: {cvDateINI:d} AL {cvDateFIN:d}, CATEGORIA: {categoriaNombre}, T.PAGO: {tipoPago}";

            return Json(new { iData = datos, titulo }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChartDxPunto(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            DateTime.TryParse(inicio, out DateTime cvDateINI);
            DateTime.TryParse(fin, out DateTime cvDateFIN);
            db.Database.CommandTimeout = 120;
            var datos = db.Database.SqlQuery<iOperacionXPunto>(
                "EXEC sp_ObtenerDistribucionPorPunto @FechaInicio, @FechaFin, @TipoOperacionID, @PuntoVentaID, @CategoriaID, @ProductoID, @TipoPagoID, @AreaID",
                new SqlParameter("@FechaInicio", cvDateINI),
                new SqlParameter("@FechaFin", cvDateFIN),
                new SqlParameter("@TipoOperacionID", toper),
                new SqlParameter("@PuntoVentaID", pventa),
                new SqlParameter("@CategoriaID", categ),
                new SqlParameter("@ProductoID", producto),
                new SqlParameter("@TipoPagoID", tpago),
                new SqlParameter("@AreaID", area)
            ).ToList();

            double? totalImporte = datos.Sum(x => x.importe);
            foreach (var item in datos)
            {
                item.porciento = (totalImporte > 0) ? (item.importe / totalImporte) * 100 : 0;
            }

            return Json(new { iData = datos }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChartDxProducto(string inicio, string fin, int toper = 0, int pventa = 0, int categ = 0, int producto = 0, int tpago = 0, int area = 0)
        {
            DateTime.TryParse(inicio, out DateTime cvDateINI);
            DateTime.TryParse(fin, out DateTime cvDateFIN);
            db.Database.CommandTimeout = 120;
            var datos = db.Database.SqlQuery<iOperacionXproducto>(
                "EXEC sp_ObtenerDistribucionPorProducto @FechaInicio, @FechaFin, @TipoOperacionID, @PuntoVentaID, @CategoriaID, @ProductoID, @TipoPagoID, @AreaID",
                new SqlParameter("@FechaInicio", cvDateINI),
                new SqlParameter("@FechaFin", cvDateFIN),
                new SqlParameter("@TipoOperacionID", toper),
                new SqlParameter("@PuntoVentaID", pventa),
                new SqlParameter("@CategoriaID", categ),
                new SqlParameter("@ProductoID", producto),
                new SqlParameter("@TipoPagoID", tpago),
                new SqlParameter("@AreaID", area)
            ).ToList();

            double? totalImporte = datos.Sum(x => x.importe);
            foreach (var item in datos)
            {
                item.porciento = (totalImporte > 0) ? (item.importe / totalImporte) * 100 : 0;
            }

            return Json(new { iData = datos }, JsonRequestBehavior.AllowGet);
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
