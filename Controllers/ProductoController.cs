using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using vercom.Models;
using EntityState = System.Data.Entity.EntityState;
using System.Data.SqlClient;
using vercom.Interfaces;

namespace vercom.Controllers
{
    [Authorize]   
    public class ProductoController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        //
        // GET: /Producto/

        [RBAC]
        public ActionResult Index()
        {       
            return View();
        }
               
        [HttpGet]
        public JsonResult GetProductos(int cantidad = 250)
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
        // GET: /Producto/Details/5

        [RBAC]
        public ActionResult Details(int id = 0)
        {
            producto producto = db.producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        //
        // GET: /Producto/Create

        [RBAC]
        public ActionResult Create()
        {
            ViewBag.categoriaid = new SelectList(db.categoria, "id", "clave", new { @class = "form-control" });
            ViewBag.unidadid = new SelectList(db.unidad, "id", "unidad1", new { @class = "form-control" });
            return View();
        }

        //
        // POST: /Producto/Create

        [HttpPost]
        [ValidateAntiForgeryToken]   
        public JsonResult Create(producto producto)
        {
            var existe = db.producto.Any(r => r.cod == producto.cod);
            if (!existe)
            {
                producto.activo = true;
                var fecha = DateTime.Now;
                producto.fecha = fecha;
                db.producto.Add(producto);
                db.SaveChanges();
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }

            return Json(new
            {
                success = false,
                message = $"El producto con el código: {producto.cod} ya existe"
            });
        }

        //
        // GET: /Producto/Edit/5

        [RBAC]
        public ActionResult Edit(int id = 0)
        {
            producto producto = db.producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        //
        // POST: /Producto/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(producto producto)
        {
            db.Entry(producto).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Producto/Delete/5
        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            bool result = false;
            var producto = db.producto.SingleOrDefault(m => m.id == id);
            if (producto != null)
            {
                db.producto.Remove(producto);
                db.SaveChanges();
                result = true;
            }
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarMultiples(int[] ids)
        {
            var productos = db.producto.Where(o => ids.Contains(o.id)).ToList();
            db.producto.RemoveRange(productos);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public ContentResult _productosGeneralData(int id, int idprod, string fecha)
        {
            List<producto> iData = new List<producto>();
            var cvFecha = Convert.ToDateTime(fecha);
            var cvFechaSaldo = cvFecha.AddDays(-1);
            var saldo = (from r in db.operacion where r.fecha == cvFechaSaldo && r.punto_ventaid == id && r.tipo_operacionid == 5 && r.productoid == idprod select r);
            var operacion = (from r in db.operacion where r.fecha == cvFecha && r.punto_ventaid == id && r.tipo_operacionid != 5 && r.productoid == idprod select r);
            double? finalSaldo = 0;
            double? precio_venta = db.producto.Where(p => p.id == idprod).Select(p => p.precio).FirstOrDefault();
            float? cantidad_entrada = 0;
            float? cantidad_venta = 0;
            float? cantidad_devolucion = 0;
            float? cantidad_merma = 0;

            foreach (var item in saldo)
            {
                var auxCe = (from r in operacion where r.tipo_operacionid == 1 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCe != null)
                {
                    cantidad_entrada = auxCe;
                }


                var auxCv = (from r in operacion where r.tipo_operacionid == 2 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCv != null)
                {
                    cantidad_venta = auxCv;
                }

                var auxCd = (from r in operacion where r.tipo_operacionid == 3 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCd != null)
                {
                    cantidad_devolucion = auxCd;
                }

                var auxCm = (from r in operacion where r.tipo_operacionid == 4 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCm != null)
                {
                    cantidad_merma = auxCm;
                }

                finalSaldo += item.cantidad + cantidad_entrada - (cantidad_venta + cantidad_devolucion + cantidad_merma);
            }

            finalSaldo = Convert.ToDouble(String.Format("{0:#,##0.00}", finalSaldo));
            producto iVal = new producto
            {
                precio = precio_venta,
                costo = finalSaldo,
            };

            iData.Add(iVal);
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        [HttpGet]
        public JsonResult ObtenerResumenProductos()
        {
            bool result = false;
            var producto = db.producto.SingleOrDefault(m => m.id == 0);
            if (producto != null)
            {
                db.producto.Remove(producto);
                db.SaveChanges();
                result = true;
            }
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}