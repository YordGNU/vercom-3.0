using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using vercom.Models;

namespace vercom.Controllers
{
    [Authorize]
    [RBAC]
    public class UnidadController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        //
        // GET: /Unidad/


        public ActionResult Index()
        {
            return View(db.unidad.ToList());
        }

        //
        // GET: /Unidad/Details/5


        public ActionResult Details(int id = 0)
        {
            unidad unidad = db.unidad.Find(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }

        //
        // GET: /Unidad/Create


        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Unidad/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(unidad unidad)
        {
            if (ModelState.IsValid)
            {
                db.unidad.Add(unidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unidad);
        }

        //
        // GET: /Unidad/Edit/5


        public ActionResult Edit(int id = 0)
        {
            unidad unidad = db.unidad.Find(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }

        //
        // POST: /Unidad/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(unidad unidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unidad);
        }

        //
        // GET: /Unidad/Delete/5


        public ActionResult Delete(int id = 0)
        {
            unidad unidad = db.unidad.Find(id);
            if (unidad == null)
            {
                return HttpNotFound();
            }
            return View(unidad);
        }

        //
        // POST: /Unidad/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            unidad unidad = db.unidad.Find(id);
            db.unidad.Remove(unidad);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}