using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using vercom.Models;

namespace vercom.Controllers
{
    [Authorize]
    [RBAC]
    public class PuntoController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        public ActionResult Index()
        {
            return View(db.punto_venta.ToList());
        }

        public ActionResult Details(int id = 0)
        {
            punto_venta punto_venta = db.punto_venta.Find(id);
            if (punto_venta == null)
            {
                return HttpNotFound();
            }
            return View(punto_venta);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(punto_venta punto_venta)
        {
            if (ModelState.IsValid)
            {
                db.punto_venta.Add(punto_venta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(punto_venta);
        }

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

        // GET: /Punto/Delete/5
        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            bool result = false;
            var punto = db.punto_venta.SingleOrDefault(m => m.id == id);
            if (punto != null)
            {
                db.punto_venta.Remove(punto);
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