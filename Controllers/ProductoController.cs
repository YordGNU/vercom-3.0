using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using vercom.Models;
using EntityState = System.Data.Entity.EntityState;

namespace vercom.Controllers
{
    [Authorize]
    [RBAC]
    public class ProductoController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        //
        // GET: /Producto/


        public ActionResult Index()
        {
            var producto = db.producto.Include(p => p.categoria).Include(p => p.unidad);
            return View(producto.ToList());
        }

        //
        // GET: /Producto/Details/5


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
        public ActionResult Create(producto producto)
        {
            var existe = (from r in db.producto where r.cod == producto.cod select r.cod).FirstOrDefault();
            if (existe == null)
            {

                db.producto.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewBag.error = "El producto con el código: " + producto.cod + " ya existe";
            ViewBag.categoriaid = new SelectList(db.categoria, "id", "clave", producto.categoriaid);
            ViewBag.unidadid = new SelectList(db.unidad, "id", "unidad1", producto.unidadid);
            return View(producto);
        }

        //
        // GET: /Producto/Edit/5


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



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}