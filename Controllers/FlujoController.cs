using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using vercom.Interfaces;
using vercom.Models;

namespace vercom.Controllers
{
    [Authorize]
    public class FlujoController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        public ActionResult Index()
        {
            var flujo = db.flujo.Include(f => f.area).Include(f => f.tipo_flujo);
            var saldo = db.presupuesto.Sum(s => s.saldo);
            ViewData["saldo"] = saldo;
            return View(flujo.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            flujo flujo = db.flujo.Find(id);
            if (flujo == null)
            {
                return HttpNotFound();
            }
            return View(flujo);
        }

        [RBAC]
        public ActionResult Create()
        {
            ViewBag.areaid = new SelectList(db.area, "id", "nombre");
            ViewBag.tipo_flujoid = new SelectList(db.tipo_flujo, "id", "tipo");
            return View();
        }

        // POST: Flujo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(flujo flujo)
        {
            if (ModelState.IsValid)
            {
                db.flujo.Add(flujo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.areaid = new SelectList(db.area, "id", "nombre", flujo.areaid);
            ViewBag.tipo_flujoid = new SelectList(db.tipo_flujo, "id", "tipo", flujo.tipo_flujoid);
            return View(flujo);
        }

        [RBAC]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            flujo flujo = db.flujo.Find(id);
            if (flujo == null)
            {
                return HttpNotFound();
            }
            ViewBag.areaid = new SelectList(db.area, "id", "nombre", flujo.areaid);
            ViewBag.tipo_flujoid = new SelectList(db.tipo_flujo, "id", "tipo", flujo.tipo_flujoid);
            return View(flujo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,areaid,tipo_flujoid,fecha,cantidad,importe,concepto")] flujo flujo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flujo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.areaid = new SelectList(db.area, "id", "nombre", flujo.areaid);
            ViewBag.tipo_flujoid = new SelectList(db.tipo_flujo, "id", "tipo", flujo.tipo_flujoid);
            return View(flujo);
        }


        [RBAC]
        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            bool result = false;
            var flujo = db.flujo.SingleOrDefault(m => m.id == id);
            if (flujo != null)
            {
                db.flujo.Remove(flujo);
                db.SaveChanges();
                result = true;
            }
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            flujo flujo = db.flujo.Find(id);
            db.flujo.Remove(flujo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [RBAC]
        public ActionResult Cierre()
        {
            return View();
        }

        public ContentResult _CierreFlujoResult(string fecha)
        {
            List<iFlujoResumen> iData = new List<iFlujoResumen>();

            var fechafilter = Convert.ToDateTime(fecha);
            var cvDateSaldo = fechafilter.AddDays(-1);

            var presupuesto = (from r in db.presupuesto where (r.fecha == cvDateSaldo) select r);
            var fdata = (from r in db.flujo where (r.fecha == fechafilter) select r);

            var cantPres = presupuesto.ToList().Count();

            var grpSaldo = (from d in fdata
                            group d by d.area.id into rp
                            select new
                            {
                                grp = rp.Key,
                                nombre = rp.Select(x => x.area.nombre).FirstOrDefault(),
                                cantidad = rp.Select(x => x.cantidad).ToList(),
                                importe = rp.Select(x => x.importe).ToList(),
                                concepto = rp.Select(x => x.concepto).ToList()
                            }).OrderBy(x => x.grp).ToList();

            foreach (var item in presupuesto)
            {
                var cantEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.importe).ToList();
                var consEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.concepto).ToList();
                var cantSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.importe).ToList();
                var consSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.concepto).ToList();

                var presupuestoArea = (from r in presupuesto where r.areaid == item.areaid select r.saldo).FirstOrDefault();
                var saldoFinal = (presupuestoArea + cantEntrada.Sum()) - (cantSalida.Sum());

                iFlujoResumen iVal = new iFlujoResumen
                {
                    areaID = item.areaid,
                    area = item.area.nombre,
                    saldo = presupuestoArea,
                    saldoFinal = saldoFinal,
                    entrada = cantEntrada.Sum(),
                    salida = cantSalida.Sum(),
                    listentrada = cantEntrada,
                    listsalida = cantSalida,
                    conceptoEntrada = consEntrada,
                    conceptoSalida = consSalida,
                };
                iData.Add(iVal);
            }

            foreach (var iter in iData)
            {
                float? saldoActual = (float?)iter.saldo;
                float? SaldoFinal = saldoActual;
                SaldoFinal += iter.entrada;
                SaldoFinal -= iter.salida;

                var saldo = new presupuesto
                {
                    fecha = fechafilter,
                    areaid = iter.areaID,
                    saldo = SaldoFinal,
                };

                db.presupuesto.Add(saldo);
                db.SaveChanges();
            }

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        public ContentResult _CierreFlujoConsultResult(string fecha)
        {
            List<iFlujoResumen> iData = new List<iFlujoResumen>();

            var fechafilter = Convert.ToDateTime(fecha);
            var cvDateSaldo = fechafilter.AddDays(-1);

            var existSAlfdate = (from s in db.presupuesto where (s.fecha == fechafilter) select s).FirstOrDefault();
            if (existSAlfdate != null)
            {
                iFlujoResumen iVal = new iFlujoResumen
                {
                    area = "CIERRE",
                    saldo = 0,
                    listentrada = null,
                    listsalida = null,
                    conceptoEntrada = null,
                    conceptoSalida = null,
                };
                iData.Add(iVal);
                return Content(JsonConvert.SerializeObject(iData), "application/json");
            }
            var presupuesto = (from r in db.presupuesto where (r.fecha == cvDateSaldo) select r);
            var fdata = (from r in db.flujo where (r.fecha == fechafilter) select r);

            var cantPres = presupuesto.ToList().Count();

            var grpSaldo = (from d in fdata
                            group d by d.area.id into rp
                            select new
                            {
                                grp = rp.Key,
                                nombre = rp.Select(x => x.area.nombre).FirstOrDefault(),
                                cantidad = rp.Select(x => x.cantidad).ToList(),
                                importe = rp.Select(x => x.importe).ToList(),
                                concepto = rp.Select(x => x.concepto).ToList()
                            }).OrderBy(x => x.grp).ToList();

            foreach (var item in presupuesto)
            {
                var cantEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.importe).ToList();
                var consEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.concepto).ToList();
                var cantSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.importe).ToList();
                var consSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.concepto).ToList();

                var presupuestoArea = (from r in presupuesto where r.areaid == item.areaid select r.saldo).FirstOrDefault();
                var saldoFinal = (presupuestoArea + cantEntrada.Sum()) - (cantSalida.Sum());


                presupuestoArea = (float?)Convert.ToDouble(String.Format("{0:#,##0.00}", presupuestoArea));
                saldoFinal = (float?)Convert.ToDouble(String.Format("{0:#,##0.00}", saldoFinal));


                iFlujoResumen iVal = new iFlujoResumen
                {
                    areaID = item.areaid,
                    area = item.area.nombre,
                    saldo = presupuestoArea,
                    saldoFinal = saldoFinal,
                    entrada = (float?)Convert.ToDouble(String.Format("{0:#,##0.00}", cantEntrada.Sum())),
                    salida = (float?)Convert.ToDouble(String.Format("{0:#,##0.00}", cantSalida.Sum())),
                    listentrada = cantEntrada,
                    listsalida = cantSalida,
                    conceptoEntrada = consEntrada,
                    conceptoSalida = consSalida,
                };
                iData.Add(iVal);
            }
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        [RBAC]
        public ActionResult Resumen()
        {
            List<iFlujoResumen> iData = new List<iFlujoResumen>();
            var fecha = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            var fechafilter = Convert.ToDateTime(fecha);
            var presupuesto = (from r in db.presupuesto where (r.fecha == fechafilter) select r);
            var fdata = (from r in db.flujo where r.fecha == fechafilter select r);
            var count = fdata.Count();

            var grpSaldo = (from d in fdata
                            group d by d.area.id into rp
                            select new
                            {
                                grp = rp.Key,
                                nombre = rp.Select(x => x.area.nombre).FirstOrDefault(),
                                cantidad = rp.Select(x => x.cantidad).ToList(),
                                importe = rp.Select(x => x.importe).ToList(),
                                concepto = rp.Select(x => x.concepto).ToList()
                            }).OrderBy(x => x.grp).ToList();

            foreach (var item in presupuesto)
            {
                var cantEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.importe).Sum();
                var consEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.concepto).ToList();
                var cantSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.importe).Sum();
                var consSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.concepto).ToList();

                var presupuestoArea = (from r in presupuesto where r.areaid == item.areaid select r.saldo).FirstOrDefault();

                iFlujoResumen iVal = new iFlujoResumen
                {
                    area = item.area.nombre,
                    saldo = presupuestoArea,
                    entrada = cantEntrada,
                    salida = cantSalida,
                    conceptoEntrada = consEntrada,
                    conceptoSalida = consSalida,
                };
                iData.Add(iVal);
            }
            return View(iData);
        }

        [HttpGet]
        public ActionResult _listFilterFlujoResumen(string fecha = "", int areaid = 0)
        {
            List<iFlujoResumen> iData = new List<iFlujoResumen>();

            var fechafilter = Convert.ToDateTime(fecha);
            var tipo = "RESUMEN";
            var presupuesto = (from r in db.presupuesto where (r.fecha == fechafilter) select r);
            var fdata = (from r in db.flujo where (r.fecha == fechafilter) select r);
            if (areaid != 0)
            {
                fdata = fdata.Where(x => x.areaid == areaid);
                tipo = "DESGLOSE";
                presupuesto = presupuesto.Where(x => x.areaid == areaid);
            }
            var cantPres = presupuesto.ToList().Count();

            var grpSaldo = (from d in fdata
                            group d by d.area.id into rp
                            select new
                            {
                                grp = rp.Key,
                                nombre = rp.Select(x => x.area.nombre).FirstOrDefault(),
                                cantidad = rp.Select(x => x.cantidad).ToList(),
                                importe = rp.Select(x => x.importe).ToList(),
                                concepto = rp.Select(x => x.concepto).ToList()
                            }).OrderBy(x => x.grp).ToList();

            foreach (var item in presupuesto)
            {
                var cantEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.importe).Sum();
                var lisEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.importe).ToList();
                var consEntrada = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 1 select d.concepto).ToList();

                var cantSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.importe).Sum();
                var listSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.importe).ToList();
                var consSalida = (from d in fdata where d.areaid == item.areaid && d.tipo_flujoid == 2 select d.concepto).ToList();

                var presupuestoArea = (from r in presupuesto where r.areaid == item.areaid select r.saldo).FirstOrDefault();


                iFlujoResumen iVal = new iFlujoResumen
                {
                    area = item.area.nombre,
                    tipo = tipo,
                    saldo = presupuestoArea,
                    entrada = cantEntrada,
                    listentrada = lisEntrada,
                    salida = cantSalida,
                    listsalida = listSalida,
                    conceptoEntrada = consEntrada,
                    conceptoSalida = consSalida,
                };
                iData.Add(iVal);
            }
            return PartialView("_listFilterFlujoResumen", iData);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
