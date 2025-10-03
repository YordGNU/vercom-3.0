using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using vercom.Interfaces;
using vercom.Models;

namespace vercom.Controllers
{
    [Authorize]    
    public class PuntoController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        [RBAC]
        public ActionResult Index()
        {
            return View(db.punto_venta.ToList());
        }

        [HttpGet]
        public JsonResult GetPuntos()
        {
            try
            {
                var puntos = db.Database.SqlQuery<punto_venta>("EXEC sp_ListadoDePuntos").ToList();
                return Json(puntos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // 🛠️ Loguear el error si tienes sistema de logs
                return Json(new { error = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(punto_venta punto_venta)
        {
            var existe = db.punto_venta.Any(r => r.nombre == punto_venta.nombre);
            if (!existe)
            {                           
                db.punto_venta.Add(punto_venta);
                db.SaveChanges();
                return Json(new { success = true});
            }

            return Json(new
            {
                success = false,
                message = $"El punto de venta con el nombre: {punto_venta.nombre} ya existe"
            });
        }

        [HttpGet]
        public JsonResult ObtenerPuntoPorId(int id)
        {
            var m = db.punto_venta.Find(id);
            if (m == null) return Json(new { exito = false, mensaje = "Punto no encontrado." }, JsonRequestBehavior.AllowGet);
            var iData = new List<iPunto>();
            iData.Add(new iPunto
            {
             PuntoID =m.id,
             Nombre = m.nombre,
            });
            return Json(new { exito = true, iData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditarPunto(int Edit_PuntoID, string Edit_Nombre)
        {
            try
            {

                var punto = db.punto_venta.Find(Edit_PuntoID);
                if (punto == null) return Json(new { success = false, message = "Punto no encontrado." });

                // Actualización de campos    
                punto.nombre = Edit_Nombre;
                db.SaveChanges();
                return Json(new { exito = true, mensaje = "Punto editado correctamente." });
            }
            catch (Exception ex)
            {
                // Log opcional
                return Json(new { success = false, message = "Error al editar: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EliminarMultiples(int[] ids)
        {
            var puntos = db.punto_venta.Where(o => ids.Contains(o.id)).ToList();
            db.punto_venta.RemoveRange(puntos);
            db.SaveChanges();
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}