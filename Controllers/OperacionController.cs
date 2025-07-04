using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using vercom.Interfaces;
using vercom.Models;
using EntityState = System.Data.Entity.EntityState;

namespace vercom.Controllers
{
    [System.Web.Mvc.Authorize]
    public class OperacionController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        [RBAC]
        public ActionResult Index()
        {

            return View(db.operacion.Take(100).ToList());
        }

        public ActionResult Details(int id = 0)
        {
            operacion operacion = db.operacion.Find(id);
            if (operacion == null)
            {
                return HttpNotFound();
            }
            return View(operacion);
        }

        public ActionResult IPV()
        {           
            return View();
        }

        public ContentResult JsonIpvResult(string inicio, string fin, int pventa = 0, int categ = 0)
        {
            var resultado = new List<iPVAnalitic>();
            var cvDateIni = DateTime.ParseExact(inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var cvDateFin = DateTime.ParseExact(fin, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var cvDateSal = cvDateIni.AddDays(-1).Date;
            var punto_venta = "";

            // 1. Obtener saldos en la fecha anterior al inicio
            var saldosInicialesQ = db.operacion
                .Where(op => op.tipo_operacionid == 5 && op.fecha == cvDateSal && op.producto != null);

            // 2. Obtener movimientos entre las fechas (1:Entrada, 2:Venta, 3:Devolución, 4:Merma)
            var movimientosQ = db.operacion
                .Where(op => op.fecha >= cvDateIni && op.fecha <= cvDateFin && op.tipo_operacionid != 5 && op.producto != null);

            // Aplicar filtros
            if (pventa != 0)
            {
                saldosInicialesQ = saldosInicialesQ.Where(op => op.punto_ventaid == pventa);
                movimientosQ = movimientosQ.Where(op => op.punto_ventaid == pventa);
                punto_venta = db.punto_venta.Where(p => p.id == pventa).Select(p => p.nombre).FirstOrDefault();
            }

            if (categ != 0)
            {
                saldosInicialesQ = saldosInicialesQ.Where(op => op.producto.categoriaid == categ);
                movimientosQ = movimientosQ.Where(op => op.producto.categoriaid == categ);
            }

            // 3. Saldo inicial agrupado
            var saldosIniciales = saldosInicialesQ.ToList()
                .GroupBy(op => op.producto.id)
                .Select(g => new {
                    ProductoId = g.Key,
                    Producto = g.First().producto,
                    Cantidad = g.Sum(op => op.cantidad ?? 0.0),
                    Importe = g.Sum(op => op.importe ?? 0.0)
                }).ToList();

            // 4. Detectar productos que aparecen por primera vez en movimientos
            var movimientos = movimientosQ.ToList();
            var productosSaldo = saldosIniciales.Select(x => x.ProductoId).ToHashSet();
            var productosEnMovimientos = movimientos.Select(m => m.producto.id).Distinct().ToList();

            var productosFaltantes = productosEnMovimientos
                .Where(id => !productosSaldo.Contains(id))
                .Select(id => {
                    var prod = movimientos.First(m => m.producto.id == id).producto;
                    return new
                    {
                        ProductoId = id,
                        Producto = prod,
                        Cantidad = 0.0,
                        Importe = 0.0
                    };
                }).ToList();

            // 5. Consolidar conjunto completo de productos
            var saldosCompletos = saldosIniciales.Concat(productosFaltantes).ToList();

            // 6. Calcular datos IPV por producto
            foreach (var saldo in saldosCompletos)
            {
                var producto = saldo.Producto;
                double precio = producto.precio ?? 0;

                double entrada = GetSum(movimientos, 1, saldo.ProductoId);
                double venta = GetSum(movimientos, 2, saldo.ProductoId);
                double devol = GetSum(movimientos, 3, saldo.ProductoId);
                double merma = GetSum(movimientos, 4, saldo.ProductoId);

                var ipv = new iPVAnalitic
                {
                    cod = producto.cod,
                    producto = producto.nombre,
                    unidad = producto.unidad?.unidad1,
                    precio_venta = precio,
                    cantidad_saldo = saldo.Cantidad,
                    importe_saldo = saldo.Cantidad * precio,
                    cantidad_entrada = entrada,
                    importe_entrada = entrada * precio,
                    cantidad_venta = venta,
                    importe_venta = venta * precio,
                    cantidad_devolucion = devol,
                    importe_devolucion = devol * precio,
                    cantidad_merma = merma,
                    importe_merma = merma * precio,
                    final_saldo = saldo.Cantidad + entrada - (venta + devol + merma),
                    final_importe = (saldo.Cantidad + entrada - (venta + devol + merma)) * precio,
                    punto_nombre = punto_venta,
                    fecha = cvDateFin
                };

                resultado.Add(ipv);
            }

            return Content(JsonConvert.SerializeObject(resultado), "application/json");
        }

        private double GetSum(List<operacion> lista, int tipoOperacion, int productoId)
        {
            return lista
                .Where(op => op.tipo_operacionid == tipoOperacion && op.producto.id == productoId)
                .Sum(op => op.cantidad ?? 0);
        }

        [RBAC]
        public ActionResult Create()
        {
            ViewBag.punto_ventaid = new SelectList(db.punto_venta, "id", "nombre");
            ViewBag.tipo_pagoid = new SelectList(db.tipo_pago, "id", "tipo");
            ViewBag.tipo_operacionid = new SelectList(db.tipo_operacion, "id", "tipo");
            ViewBag.productoid = new SelectList(db.producto, "id", "cod");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(operacion operacion, List<iProductoOperacion> productos)
        {
            var cvDateSal = operacion.fecha.Value.AddDays(-1);
            var userId = db.Users.Where(u => u.UserName == HttpContext.User.Identity.Name).Select(u => u.UserID).FirstOrDefault();
            var Rol = db.UserRoles.Where(u => u.Users.UserName == HttpContext.User.Identity.Name).Select(u => u.Roles.RoleName).FirstOrDefault();

            var notificacionesParaGuardar = new List<Notifications>();
            var operacionesParaGuardar = new List<operacion>();
            var trazasParaGuardar = new List<trazas>(); // Lista para trazas


            // 🔥 Optimizar consultas: Obtener todos los productos en un diccionario
            var productosDB = db.producto.ToDictionary(x => x.id);

            foreach (var item in productos)
            {

                if (!productosDB.ContainsKey(item.ProductoID)) continue; // Evita registros inválidos

                var producto = productosDB[item.ProductoID];
                var categoriaID = producto.categoriaid;
                var precio = producto.precio;

                var existeCierreCategoria = db.operacion.Any(r =>
                r.punto_ventaid == operacion.punto_ventaid &&
                r.fecha == operacion.fecha &&
                r.tipo_operacionid == 5 &&
                r.producto.categoriaid == categoriaID);

                if (existeCierreCategoria)
                {
                    ViewBag.error = $"No se pueden agregar operaciones a un punto de venta si ya cerró en la fecha: {operacion.fecha.Value:d}";
                    return View(operacion);
                }

                var puntoVenta = db.punto_venta.Find(operacion.punto_ventaid);
                if (puntoVenta == null) continue;

                // 🔥 Corrección de formato numérico
                var cantidadStr = item.Cantidad?.ToString().Replace(",", ".");
                var importeStr = item.Importe?.ToString().Replace(",", ".");

                var cantidad = string.IsNullOrWhiteSpace(cantidadStr) ? (float?)null : (float?)Convert.ToDecimal(cantidadStr, CultureInfo.InvariantCulture);
                var importe = string.IsNullOrWhiteSpace(importeStr) ? (float?)null : (float?)Convert.ToDecimal(importeStr, CultureInfo.InvariantCulture);

                if (cantidad == null)
                {
                    ViewBag.error = "Error: La cantidad ingresada no es válida.";
                    return View(operacion);
                }

                var saldo = db.operacion.FirstOrDefault(r =>
                    r.punto_ventaid == operacion.punto_ventaid &&
                    r.productoid == item.ProductoID &&
                    r.tipo_operacionid == 5 &&
                    r.fecha == cvDateSal);



                var nuevaOperacion = new operacion
                {
                    cantidad = cantidad,
                    fecha = operacion.fecha,
                    importe = importe,
                    productoid = item.ProductoID,
                    punto_ventaid = operacion.punto_ventaid,
                    tipo_operacionid = operacion.tipo_operacionid,
                    tipo_pagoid = operacion.tipo_operacionid == 2 ? (int?)item.TipoPagoID : null
                };

                operacionesParaGuardar.Add(nuevaOperacion);

                var mensaje = $"Nueva operación de SALDO para el producto {producto.nombre} - {producto.cod} en el punto de venta {puntoVenta.nombre}";

                notificacionesParaGuardar.Add(new Notifications
                {
                    UserId = userId,
                    Message = mensaje,
                    CreatedAt = DateTime.Now,
                    Role = Rol,
                    IsRead = false
                });

                if (saldo == null && operacion.tipo_operacionid != 5)
                {
                    var obj_saldo = new operacion
                    {
                        cantidad = 0,
                        fecha = cvDateSal,
                        importe = 0,
                        productoid = item.ProductoID,
                        punto_ventaid = operacion.punto_ventaid,
                        tipo_operacionid = 5,
                    };
                    operacionesParaGuardar.Add(obj_saldo);
                }

                // 🔥 Agregar trazas simultáneas
                var trz = new trazas
                {
                    fecha = DateTime.Now,
                    usuario = HttpContext.User.Identity.Name,
                    accion = "Registro de Operación",
                    descripcion = mensaje,
                    ip = Request.UserHostAddress,
                    modulo = "Operaciones"
                };
                db.trazas.Add(trz);

            }

            // 🔥 Guardar todas las operaciones y notificaciones en un solo `SaveChanges()`
            db.operacion.AddRange(operacionesParaGuardar);
            db.SaveChanges();

            // 📢 Enviar notificación en tiempo real con SignalR
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            foreach (var notificacion in notificacionesParaGuardar)
            {
                hubContext.Clients.All.RecibirNotificacion(notificacion);
            }

            return RedirectToAction("Index");
        }

        [RBAC]
        public ActionResult Cierre()
        {
            ViewBag.punto_ventaid = new SelectList(db.punto_venta, "id", "nombre");
            ViewBag.categoriaid = new SelectList(db.categoria, "id", "clave");
            return View();
        }

        public ContentResult CierreJsonResult(string fecha, int pventa, int categ)
        {
            var iData = new List<iPVAnalitic>();
            var cvDateIni = Convert.ToDateTime(fecha);
            var cvDateSal = cvDateIni.Date.AddDays(-1);

            var fsaldo = db.operacion.Where(r => r.punto_ventaid == pventa && r.tipo_operacionid == 5 && r.fecha == cvDateSal && r.producto.categoriaid == categ);
            var fdate = db.operacion.Where(d => d.punto_ventaid == pventa && d.fecha == cvDateIni && d.producto.categoriaid == categ);

            foreach (var isaldo in fsaldo)
            {
                var producto = isaldo.producto.nombre;
                var unidad = isaldo.producto.unidad.unidad1;
                var precio_venta = (double)isaldo.producto.precio;

                double cantidad_entrada = fdate.Where(r => r.tipo_operacionid == 1 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;
                double cantidad_venta = fdate.Where(r => r.tipo_operacionid == 2 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;
                double cantidad_devolucion = fdate.Where(r => r.tipo_operacionid == 3 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;
                double cantidad_merma = fdate.Where(r => r.tipo_operacionid == 4 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;

                double finalSaldo = (double)(isaldo.cantidad + cantidad_entrada - (cantidad_venta + cantidad_devolucion + cantidad_merma));
                double finalImporte = (double)(isaldo.importe + (cantidad_entrada * precio_venta) - ((cantidad_venta + cantidad_devolucion + cantidad_merma) * precio_venta));

                var iVal = new iPVAnalitic
                {
                    id = isaldo.producto.id,
                    cod = isaldo.producto.cod,
                    producto = producto,
                    fecha = (DateTime)isaldo.fecha,
                    unidad = unidad,
                    final_saldo = finalSaldo,
                    final_importe = finalImporte
                };
                iData.Add(iVal);
            }

            foreach (var iter in iData)
            {
                var existe = db.operacion.Any(o => o.fecha == cvDateIni && o.productoid == iter.id && o.punto_ventaid == pventa && o.tipo_operacionid == 5);
                if (!existe)
                {
                    var oper = new operacion
                    {
                        fecha = cvDateIni,
                        cantidad = (float)iter.final_saldo,
                        importe = (float)iter.final_importe,
                        productoid = iter.id,
                        punto_ventaid = pventa,
                        tipo_operacionid = 5
                    };
                    db.operacion.Add(oper);
                    db.SaveChanges();
                }
            }

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        public ContentResult CierreJsonConsultResult(string fecha, int pventa, int categ)
        {
            var iData = new List<iPVAnalitic>();
            var cvDateIni = Convert.ToDateTime(fecha);
            var cvDateSal = cvDateIni.Date.AddDays(-1);

            var fdate = db.operacion.Where(d => d.punto_ventaid == pventa && d.fecha == cvDateIni && d.producto.categoriaid == categ).ToList();
            var existSAlfdate = fdate.FirstOrDefault(s => s.tipo_operacionid == 5);

            if (existSAlfdate != null)
            {
                iData.Add(new iPVAnalitic
                {
                    cod = "YA EXISTE",
                    producto = "CIERRE",
                    fecha = cvDateIni,
                    unidad = "ACT. SALDO EN SU LUGAR",
                    final_saldo = 0,
                    final_importe = 0,
                    id = 0,
                    lnk_entrada = 0,
                    lnk_venta = 0,
                    lnk_devol = 0,
                    lnk_merma = 0,
                    cantidad_saldo = 0,
                    importe_saldo = 0,
                    cantidad_entrada = 0,
                    importe_entrada = 0,
                    cantidad_venta = 0,
                    precio_venta = 0,
                    importe_venta = 0,
                    cantidad_devolucion = 0,
                    importe_devolucion = 0,
                    cantidad_merma = 0,
                    importe_merma = 0,

                });

                return Content(JsonConvert.SerializeObject(iData), "application/json");
            }

            var fsaldo = db.operacion.Where(r => r.punto_ventaid == pventa && r.tipo_operacionid == 5 && r.fecha == cvDateSal && r.producto.categoriaid == categ).ToList();

            foreach (var isaldo in fsaldo)
            {
                var producto = isaldo.producto.nombre;
                var unidad = isaldo.producto.unidad.unidad1;
                var precio_venta = (double)isaldo.producto.precio;

                var cantidad_entrada = fdate.Where(r => r.tipo_operacionid == 1 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;
                var lnk_entrada = cantidad_entrada > 0 ? fdate.Where(r => r.tipo_operacionid == 1 && r.productoid == isaldo.productoid).Select(r => r.id).FirstOrDefault() : 0;
                var cantidad_venta = fdate.Where(r => r.tipo_operacionid == 2 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;
                var lnk_venta = cantidad_venta > 0 ? fdate.Where(r => r.tipo_operacionid == 2 && r.productoid == isaldo.productoid).Select(r => r.id).FirstOrDefault() : 0;
                var cantidad_devolucion = fdate.Where(r => r.tipo_operacionid == 3 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;
                var lnk_devol = cantidad_devolucion > 0 ? fdate.Where(r => r.tipo_operacionid == 3 && r.productoid == isaldo.productoid).Select(r => r.id).FirstOrDefault() : 0;
                var cantidad_merma = fdate.Where(r => r.tipo_operacionid == 4 && r.productoid == isaldo.productoid).Sum(r => r.cantidad) ?? 0;
                var lnk_merma = cantidad_merma > 0 ? fdate.Where(r => r.tipo_operacionid == 4 && r.productoid == isaldo.productoid).Select(r => r.id).FirstOrDefault() : 0;

                var finalSaldo = isaldo.cantidad + cantidad_entrada - (cantidad_venta + cantidad_devolucion + cantidad_merma);
                var finalImporte = finalSaldo * precio_venta;


                iData.Add(new iPVAnalitic
                {
                    id = isaldo.id,
                    cod = isaldo.producto.cod,
                    lnk_entrada = lnk_entrada,
                    lnk_venta = lnk_venta,
                    lnk_devol = lnk_devol,
                    lnk_merma = lnk_merma,
                    producto = producto,
                    fecha = (DateTime)isaldo.fecha,
                    unidad = unidad,
                    cantidad_saldo = Convert.ToDouble(String.Format("{0:#,##0.00}", isaldo.cantidad)),
                    importe_saldo = isaldo.cantidad * precio_venta,
                    cantidad_entrada = Convert.ToDouble(String.Format("{0:#,##0.00}", cantidad_entrada)),
                    importe_entrada = cantidad_entrada * precio_venta,
                    cantidad_venta = Convert.ToDouble(String.Format("{0:#,##0.00}", cantidad_venta)),
                    precio_venta = precio_venta,
                    importe_venta = cantidad_venta * precio_venta,
                    cantidad_devolucion = Convert.ToDouble(String.Format("{0:#,##0.00}", cantidad_devolucion)),
                    importe_devolucion = cantidad_devolucion * precio_venta,
                    cantidad_merma = Convert.ToDouble(String.Format("{0:#,##0.00}", cantidad_merma)),
                    importe_merma = cantidad_merma * precio_venta,
                    final_saldo = Convert.ToDouble(String.Format("{0:#,##0.00}", finalSaldo)),
                    final_importe = finalImporte,
                });
            }
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        [RBAC]
        public ActionResult Edit(int id = 0)
        {
            operacion operacion = db.operacion.Find(id);
            if (operacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.punto_ventaid = new SelectList(db.punto_venta, "id", "nombre", operacion.punto_ventaid);
            ViewBag.tipo_pagoid = new SelectList(db.tipo_pago, "id", "tipo", operacion.tipo_pagoid);
            ViewBag.tipo_operacionid = new SelectList(db.tipo_operacion, "id", "tipo", operacion.tipo_operacionid);
            ViewBag.productoid = new SelectList(db.producto, "id", "cod", operacion.productoid);
            return View(operacion);
        }

        [HttpPost]
        public ActionResult Edit(operacion operacion)
        {
            var precio = db.producto.Where(x => x.id == operacion.productoid).Select(x => x.precio).FirstOrDefault();

            if (operacion.importe == null)
            {
                operacion.importe = (float?)(operacion.cantidad * precio);
            }

            var mensaje = $"Editar operación {operacion.id} producto {operacion.productoid}  punto de venta {operacion.punto_ventaid}";

            var trz = new trazas
            {
                fecha = DateTime.Now,
                usuario = HttpContext.User.Identity.Name,
                accion = "Editar Operación",
                descripcion = mensaje,
                ip = Request.UserHostAddress,
                modulo = "Operaciones"
            };

            db.trazas.Add(trz);
            db.Entry(operacion).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            bool result = false;
            var operacion = db.operacion.SingleOrDefault(m => m.id == id);

            if (operacion != null)
            {
                var mensaje = $"Operación de {operacion.tipo_operacion.tipo}  {operacion.producto.nombre} - {operacion.producto.cod} en el punto de venta {operacion.punto_venta.nombre}";
                // 🔥 Agregar trazas simultáneas
                var trz = new trazas
                {
                    fecha = DateTime.Now,
                    usuario = HttpContext.User.Identity.Name,
                    accion = "Eliminar Operación",
                    descripcion = mensaje,
                    ip = Request.UserHostAddress,
                    modulo = "Operaciones"
                };
                db.trazas.Add(trz);
                db.operacion.Remove(operacion);
                db.SaveChanges();
                result = true;
            }
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarMultiples(int[] ids)
        {
            var operaciones = db.operacion.Where(o => ids.Contains(o.id)).ToList();
            db.operacion.RemoveRange(operaciones);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public ActionResult _filterOperacion()
        {
            return PartialView();
        }

        public ActionResult _filterRangeOperacion()
        {
            return PartialView();
        }

        public ContentResult filterOperacionResult(string fecha = "", int tperacion = 0, int tpago = 0, int pventa = 0, int prod = 0)
        {
            var iData = new List<iOperacion>();

            var oper = (from o in db.operacion select o);
            if (fecha != "") { var cvdate = Convert.ToDateTime(fecha); oper = oper.Where(x => x.fecha == cvdate); }
            if (tperacion != 0) { oper = oper.Where(x => x.tipo_operacionid == tperacion); }
            if (tpago != 0) { oper = oper.Where(x => x.tipo_pagoid == tpago); }
            if (pventa != 0) { oper = oper.Where(x => x.punto_ventaid == pventa); }
            if (prod != 0) { oper = oper.Where(x => x.productoid == prod); }

            foreach (var item in oper.ToList())
            {
                var iItem = new iOperacion
                {
                    id = item.id,
                    cantidad = Convert.ToDouble(String.Format("{0:#,##0.000}", item.cantidad)),
                    unidad = item.producto.unidad.unidad1,
                    fecha = item.fecha.Value.ToString("d"),
                    importe = Convert.ToDouble(String.Format("{0:#,##0.000}", item.importe)),
                    prod_cod = item.producto.cod,
                    prod_nombre = item.producto.nombre,
                    punto_venta = item.punto_venta.nombre,
                    tipo_operacion = item.tipo_operacion.tipo,
                };

                iData.Add(iItem);
            }

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        [HttpGet]
        public ActionResult _listFilterOperacion(string fecha = "", int tperacion = 0, int tpago = 0, int pventa = 0, int categ = 0, int prod = 0)
        {
            var oper = (from o in db.operacion select o);
            if (fecha != "") { var cvdate = Convert.ToDateTime(fecha); oper = oper.Where(x => x.fecha == cvdate); }
            if (tperacion != 0) { oper = oper.Where(x => x.tipo_operacionid == tperacion); }
            if (tpago != 0) { oper = oper.Where(x => x.tipo_pagoid == tpago); }
            if (pventa != 0) { oper = oper.Where(x => x.punto_ventaid == pventa); }
            if (categ != 0) { oper = oper.Where(x => x.producto.categoriaid == categ); }
            if (prod != 0) { oper = oper.Where(x => x.productoid == prod); }
            return PartialView("_listFilterOperacion", oper.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}