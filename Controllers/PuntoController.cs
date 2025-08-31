using System;
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

        [RBAC]
        public ActionResult Details(int id = 0)
        {
            punto_venta punto_venta = db.punto_venta.Find(id);
            if (punto_venta == null)
            {
                return HttpNotFound();
            }
            return View(punto_venta);
        }

        [RBAC]
        public ActionResult Create()
        {
            return View();
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
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }

            return Json(new
            {
                success = false,
                message = $"El punto de venta con el nombre: {punto_venta.nombre} ya existe"
            });
        }

        [RBAC]
        public ActionResult Edit(int id = 0)
        {
            punto_venta punto_venta = db.punto_venta.Find(id);
            if (punto_venta == null)
            {
                return HttpNotFound();
            }
            return View(punto_venta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(punto_venta punto_venta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(punto_venta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(punto_venta);
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