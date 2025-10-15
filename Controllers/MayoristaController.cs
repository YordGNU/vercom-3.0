using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using vercom.Interfaces;
using vercom.Models;
using EntityState = System.Data.Entity.EntityState;

namespace vercom.Controllers
{
    [System.Web.Mvc.Authorize]
    public class MayoristaController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();
        private static List<cuenta> elementos = new List<cuenta>();

        [RBAC]
        public ActionResult Index()
        {
            // Obtener todos los datos necesarios en una sola consulta
            var ftdata = db.negocio.Include(n => n.cliente).Include(n => n.forma_operacion).Include(n => n.medio_pago).Include(n => n.tipo_factura).ToList();
            // Calcular valores totales
            var TotalImpVentas = ftdata.Sum(s => s.producto_servicio.precio * s.cantidad);
            var TotalCostVentas = ftdata.Sum(s => s.producto_servicio.costo * s.cantidad);
            var totalCantidad = ftdata.Sum(x => x.cantidad);
            var TotalUtilidades = TotalImpVentas - TotalCostVentas;
            var total_cantventas = ftdata.Count;
         

            // Calcular valores de pago
            decimal tVefectivo = ftdata.Count(sx => sx.medio_pago.id == 1);
            decimal tVtransfer = ftdata.Count(sx => sx.medio_pago.id == 2);
            var modaClienteNombre = ftdata.GroupBy(r => r.cliente.nombre)
                .Select(grp => new {
                    Nombre = grp.Key,
                    Frecuencia = grp.Count()
                }).OrderByDescending(x => x.Frecuencia).Select(x => x.Nombre) // Selecciona solo el nombre
                .FirstOrDefault();
            var modaProductoServi = ftdata.GroupBy(r => r.producto_servicio.nombre)
                .Select(grp => new {
                    Nombre = grp.Key,
                    Frecuencia = grp.Count()
                }).OrderByDescending(x => x.Frecuencia).Select(x => x.Nombre) // Selecciona solo el nombre
                .FirstOrDefault();
            var modaFormaOperacion = ftdata.GroupBy(r => r.forma_operacion.forma)
                .Select(grp => new {
                    Nombre = grp.Key,
                    Frecuencia = grp.Count()
                }).OrderByDescending(x => x.Frecuencia).Select(x => x.Nombre) // Selecciona solo el nombre
                .FirstOrDefault();



            // Formatear y asignar valores a ViewData
            ViewData["total_impventas"] = Convert.ToDecimal(String.Format("{0:#,##0.00}", TotalImpVentas));           
            ViewData["total_costventas"] = Convert.ToDouble(String.Format("{0:#,##0.00}", TotalCostVentas));        
            ViewData["utilidades"] = Convert.ToDouble(String.Format("{0:#,##0.00}", TotalUtilidades));
       
            ViewData["modaCliente"] = modaClienteNombre;
            ViewData["modaServiProducto"] = modaProductoServi; 
            ViewData["modaFormaOperacion"] = modaFormaOperacion; 


            // Calcular distribuciones
            var dist_vcliente = ftdata.GroupBy(r => r.cliente.nombre)
                .Select(rp => new {
                    grp = rp.Key,
                    Count = rp.Count(),
                    Cantidad = rp.Sum(x => x.cantidad),
                    Utilidad = rp.Sum(x => x.cantidad * (x.producto_servicio.precio - x.producto_servicio.costo))
                }).OrderBy(x => x.grp).ToList();

            var dist_cventa = ftdata.GroupBy(r => r.forma_operacion.forma)
                .Select(rp => new {
                    grp = rp.Key,
                    Count = rp.Count(),
                    Cantidad = rp.Sum(x => x.cantidad),
                    Utilidad = rp.Sum(x => x.cantidad * (x.producto_servicio.precio - x.producto_servicio.costo)),
                    Porcentaje = total_cantventas > 0 ? (rp.Count() / (double) total_cantventas) * 100 : 0
                }).OrderBy(x => x.grp).ToList();

            var dist_mpago = ftdata.GroupBy(r => r.medio_pago.medio)
               .Select(rp => new {
                   grp = rp.Key,                 
                   Porcentaje = total_cantventas > 0 ? (rp.Count() / (double)total_cantventas) * 100 : 0
               }).OrderBy(x => x.grp).ToList();
    

            var dist_tcliente = ftdata.GroupBy(r => r.cliente.tipo_cliente.tipo)
               .Select(rp => new {
                   grp = rp.Key,                  
                   Porcentaje = total_cantventas > 0 ? (rp.Count() / (double)total_cantventas) * 100 : 0
               }).OrderBy(x => x.grp).ToList();

            ViewData["dist_vcliente"] = JsonConvert.SerializeObject(dist_vcliente);
            ViewData["dist_cventa"] = JsonConvert.SerializeObject(dist_cventa);
            ViewData["dist_mpago"] = JsonConvert.SerializeObject(dist_mpago);
            ViewData["dist_tcliente"] = JsonConvert.SerializeObject(dist_tcliente);
          
            if (User.Identity.IsAuthenticated)
            {
                // Verificar si Quartz ya ha sido iniciado en esta sesión
                if (Session["QuartzIniciado"] == null)
                {
                    Task.Run(() => QuartzScheduler.IniciarScheduler());
                    // Marcar que Quartz ya se ejecutó en esta sesión
                    Session["QuartzIniciado"] = true;
                }
            }
            return View();
        }
        // INDEX NEGOCIO, CLEINTE, PRODUCTOS y SERVICIOS
        [RBAC]
        public ActionResult Negocio()
        {
            var negocio = db.negocio.Include(n => n.cliente).Include(n => n.forma_operacion).Include(n => n.medio_pago).Include(n => n.tipo_factura);
            return View(negocio.ToList());
        }
        [RBAC]
        public ActionResult Cliente()
        {
            var cliente = db.cliente.Include(c => c.tipo_cliente);
            return View(cliente.ToList());
        }
        [RBAC]
        public ActionResult ProductoServi()
        {           
            return View();
        }

        [HttpGet]
        public JsonResult GetProductoServi(int cantidad = 1000)
        {
            try
            {
                var iData = new List<iProductoServi>();
                var productosServi = db.producto_servicio.OrderByDescending(p => p.id).Take(cantidad).ToList();
                foreach (var item in productosServi) {
                    iData.Add(new iProductoServi { 
                        ProductoID = item.id,
                        Nombre = item.nombre,
                        Precio = item.precio,
                        Costo = item.costo,                    
                    });
                
                }
                return Json(iData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // 🛠️ Loguear el error si tienes sistema de logs
                return Json(new { error = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateProductoServi(producto_servicio producto)
        {
            var existe = db.producto_servicio.Any(r => r.nombre == producto.nombre);
            if (!existe)
            {              
                db.producto_servicio.Add(producto);
                db.SaveChanges();
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }

            return Json(new
            {
                success = false,
                message = $"El producto: {producto.nombre} ya existe"
            });
        }


        [HttpPost] 
        public JsonResult EditarProductoServi(int Edit_ProductoID, string Edit_Nombre, decimal Edit_Precio, decimal Edit_Costo)
        {
            try
            {

                var productoServi = db.producto_servicio.Find(Edit_ProductoID);
                if (productoServi == null)
                    return Json(new { success = false, message = "Producto/Servicio no encontrado." });

                // Actualización de campos              
                productoServi.nombre = Edit_Nombre;
                productoServi.precio = (float?) Edit_Precio;
                productoServi.costo = (float?) Edit_Costo; 
                db.SaveChanges();
                return Json(new { exito = true, mensaje = "Producto/Servicio editado correctamente." });
            }
            catch (Exception ex)
            {
                // Log opcional
                return Json(new { success = false, message = "Error al editar: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerProductoServiPorId(int id)
        {
            var m = db.producto_servicio.Find(id);
            if (m == null) return Json(new { exito = false, mensaje = "Producto/Servicio no encontrado." }, JsonRequestBehavior.AllowGet);
            var iData = new List<iProductoServi>();
            iData.Add(new iProductoServi{
            
                ProductoID = m.id,
                Nombre = m.nombre,               
                Precio = m.precio,
                Costo = m.costo
              
            });

            return Json(new { exito = true, iData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarMultiplesProductoServi(int[] ids)
        {
            var productosServi = db.producto_servicio.Where(o => ids.Contains(o.id)).ToList();
            db.producto_servicio.RemoveRange(productosServi);
            db.SaveChanges();
            return Json(new { success = true });
        }            

        //CREAR CLIENTE 
        [RBAC]
        public ActionResult CreateCliente()
        {
            ViewBag.tipoClienteID = new SelectList(db.tipo_cliente, "id", "tipo");
            return View();                 
        }

        // Asegúrate de importar esta librería
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCliente(cliente cliente, List<cuenta> cuentas)
        {
        using (var db = new VERCOMEntities())
        {
            if (cliente == null || cuentas == null || cuentas.Count == 0)
            {
                ModelState.AddModelError("", "Debe ingresar al menos una cuenta bancaria.");
                return View(cliente);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Guardar el cliente
                    db.cliente.Add(cliente);
                    db.SaveChanges();

                    // Guardar las cuentas y asociarlas con el cliente
                    List<cliente_cuenta> asociaciones = new List<cliente_cuenta>();
                    foreach (var cuenta in cuentas)
                    {
                        db.cuenta.Add(cuenta);
                        db.SaveChanges();

                        asociaciones.Add(new cliente_cuenta
                        {
                            clienteid = cliente.id,
                            cuentaid = cuenta.id
                        });
                    }

                    // Guardar asociaciones
                    db.cliente_cuenta.AddRange(asociaciones);
                    db.SaveChanges();

                    transaction.Commit();
                    ViewBag.SuccessMessage = "Cliente y cuentas bancarias registradas exitosamente.";
                    return RedirectToAction("Cliente");
                }
                catch (DbEntityValidationException ex)
                {
                    transaction.Rollback();

                    // Capturar detalles de los errores
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError("", $"Propiedad: {error.PropertyName} - Error: {error.ErrorMessage}");
                        }
                    }

                    return View(cliente);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Ocurrió un error inesperado: " + ex.Message);
                    return View(cliente);
                }
            }
        }        
        }
        //EDITAR CLIENTE
        [RBAC]
        public ActionResult DetailsCliente(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cliente cliente = db.cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        [RBAC]
        public ActionResult EditCliente(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cliente cliente = db.cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.tipoClienteID = new SelectList(db.tipo_cliente, "id", "tipo", cliente.tipoClienteID);
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCliente(cliente cliente, List<cuenta> cuentas)
        {
            using (var db = new VERCOMEntities())
            {
                if (cliente == null || cuentas == null)
                {
                    ModelState.AddModelError("", "Debe ingresar al menos una cuenta bancaria.");
                    return View(cliente);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 🔥 Actualizar los datos del cliente
                        db.Entry(cliente).State = EntityState.Modified;
                        db.SaveChanges();

                        // ✅ Obtener todas las cuentas actuales del cliente
                        var cuentasActuales = db.cliente_cuenta.Where(cc => cc.clienteid == cliente.id).Select(cc => cc.cuentaid).ToList();

                        // 🔥 Identificar cuentas eliminadas
                        var idsCuentasRecibidas = cuentas.Select(c => c.id).ToList();
                        var cuentasAEliminar = cuentasActuales.Select(c => c.Value).Except(idsCuentasRecibidas).ToList();

                        // ✅ Eliminar relaciones cliente-cuenta para cuentas eliminadas
                        foreach (var cuentaId in cuentasAEliminar)
                        {
                            var relacion = db.cliente_cuenta.FirstOrDefault(cc => cc.clienteid == cliente.id && cc.cuentaid == cuentaId);
                            if (relacion != null)
                            {
                                db.cliente_cuenta.Remove(relacion);
                            }
                        }
                        db.SaveChanges();

                        // 🔥 Verificar cuentas nuevas y mantener existentes
                        foreach (var cuenta in cuentas)
                        {
                            var cuentaExistente = db.cuenta.FirstOrDefault(c => c.no == cuenta.no);

                            if (cuentaExistente == null)
                            {
                                db.cuenta.Add(cuenta);
                                db.SaveChanges();
                                cuentaExistente = cuenta;
                            }

                            var asociacionExiste = db.cliente_cuenta.Any(a => a.clienteid == cliente.id && a.cuentaid == cuentaExistente.id);

                            if (!asociacionExiste)
                            {
                                db.cliente_cuenta.Add(new cliente_cuenta
                                {
                                    clienteid = cliente.id,
                                    cuentaid = cuentaExistente.id
                                });
                            }
                        }
                        db.SaveChanges();

                        transaction.Commit();
                        ViewBag.SuccessMessage = "Cliente y cuentas actualizadas exitosamente.";
                        return RedirectToAction("Cliente");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Ocurrió un error inesperado: " + ex.Message);
                        return View(cliente);
                    }
                }
            }
        }

        //CREAR NEGOCIO
        [RBAC]
        public ActionResult CreateNegocio()
        {
            ViewBag.productoID = new SelectList(db.producto_servicio, "id", "nombre", "precio");
            ViewBag.clienteID = new SelectList(db.cliente, "id", "nombre");
            ViewBag.operacionID = new SelectList(db.forma_operacion, "id", "forma");
            ViewBag.medioPagoID = new SelectList(db.medio_pago, "id", "medio");
            ViewBag.facturaID = new SelectList(db.tipo_factura, "id", "tipo");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNegocio([Bind(Include = "id,fecha,productoID,clienteID,cantidad,operacionID,facturaID,medioPagoID,factura")] negocio negocio)
        {
           if(negocio.productoID != null)
            {
                db.negocio.Add(negocio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }               
            
            ViewBag.productoID = new SelectList(db.producto_servicio, "id", "nombre");
            ViewBag.clienteID = new SelectList(db.cliente, "id", "nombre", negocio.clienteID);
            ViewBag.operacionID = new SelectList(db.forma_operacion, "id", "forma", negocio.operacionID);
            ViewBag.medioPagoID = new SelectList(db.medio_pago, "id", "medio", negocio.medioPagoID);
            ViewBag.facturaID = new SelectList(db.tipo_factura, "id", "tipo", negocio.facturaID);
            return View(negocio);
        }

        [RBAC]
        public ActionResult DetailsNegocio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            negocio negocio = db.negocio.Find(id);
            if (negocio == null)
            {
                return HttpNotFound();
            }
            return View(negocio);
        }

        //EDITAR NEGOCIO
        [RBAC]
        public ActionResult EditNegocio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            negocio negocio = db.negocio.Find(id);
            if (negocio == null)
            {
                return HttpNotFound();
            }
            ViewBag.productoID = new SelectList(db.producto_servicio, "id", "nombre");
            ViewBag.clienteID = new SelectList(db.cliente, "id", "nombre", negocio.clienteID);
            ViewBag.operacionID = new SelectList(db.forma_operacion, "id", "forma", negocio.operacionID);
            ViewBag.medioPagoID = new SelectList(db.medio_pago, "id", "medio", negocio.medioPagoID);
            ViewBag.facturaID = new SelectList(db.tipo_factura, "id", "tipo", negocio.facturaID);
            return View(negocio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNegocio([Bind(Include = "id,fecha,productoID,clienteID,cantidad,operacionID,facturaID,medioPagoID,factura")] negocio negocio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.productoID = new SelectList(db.producto_servicio, "id", "nombre");
            ViewBag.clienteID = new SelectList(db.cliente, "id", "nombre", negocio.clienteID);
            ViewBag.operacionID = new SelectList(db.forma_operacion, "id", "forma", negocio.operacionID);
            ViewBag.medioPagoID = new SelectList(db.medio_pago, "id", "medio", negocio.medioPagoID);
            ViewBag.facturaID = new SelectList(db.tipo_factura, "id", "tipo", negocio.facturaID);
            return View(negocio);
        }

        public ActionResult _filterMayorista()
        {
            ViewBag.productoID = new SelectList(db.producto_servicio, "id", "nombre", "precio");
            ViewBag.clienteID = new SelectList(db.cliente, "id", "nombre");
            ViewBag.operacionID = new SelectList(db.forma_operacion, "id", "forma");
            ViewBag.medioPagoID = new SelectList(db.medio_pago, "id", "medio");
            ViewBag.facturaID = new SelectList(db.tipo_factura, "id", "tipo");
            return PartialView();
        }

        [HttpGet]
        public ContentResult MayoristaGeneralData(string inicio, string fin, int productoID, int clienteID, int operacionID, int facturaID, int medioPagoID, string factura)
        {
            List<iGeneralMayoristaData> iData = new List<iGeneralMayoristaData>();

            DateTime cvDateINI = Convert.ToDateTime(inicio);
            DateTime cvDateFIN = Convert.ToDateTime(fin);

            // Obtener todos los datos necesarios en una sola consulta
            var ftdata = db.negocio.Include(n => n.cliente).Include(n => n.forma_operacion).Include(n => n.medio_pago).Include(n => n.tipo_factura).Where(n => n.fecha >= cvDateINI & n.fecha <= cvDateFIN);

            if (productoID != 0) { ftdata = ftdata.Where(x => x.productoID == productoID); }
            if (clienteID != 0) { ftdata = ftdata.Where(x => x.clienteID == clienteID); }
            if (operacionID != 0) { ftdata = ftdata.Where(x => x.operacionID == operacionID); }
            if (facturaID != 0) { ftdata = ftdata.Where(x => x.facturaID == facturaID); }
            if (medioPagoID != 0) { ftdata = ftdata.Where(x => x.medioPagoID == medioPagoID); }
            if (factura != "") { ftdata = ftdata.Where(x => x.factura == factura); }

            // Calcular valores totales
            var TotalImpVentas = ftdata.Sum(s => s.producto_servicio.precio * s.cantidad);
            var TotalCostVentas = ftdata.Sum(s => s.producto_servicio.costo * s.cantidad);
            var totalCantidad = ftdata.Sum(x => x.cantidad);
            var TotalUtilidades = TotalImpVentas - TotalCostVentas;
            var total_cantventas = ftdata.ToList().Count;


            // Calcular valores de pago
            decimal tVefectivo = ftdata.Count(sx => sx.medio_pago.id == 1);
            decimal tVtransfer = ftdata.Count(sx => sx.medio_pago.id == 2);

            var modaClienteNombre = ftdata.GroupBy(r => r.cliente.nombre)
                .Select(grp => new {
                    Nombre = grp.Key,
                    Frecuencia = grp.Count()
                }).OrderByDescending(x => x.Frecuencia).Select(x => x.Nombre) // Selecciona solo el nombre
                .FirstOrDefault();

            var modaProductoServi = ftdata.GroupBy(r => r.producto_servicio.nombre)
                .Select(grp => new {
                    Nombre = grp.Key,
                    Frecuencia = grp.Count()
                }).OrderByDescending(x => x.Frecuencia).Select(x => x.Nombre) // Selecciona solo el nombre
                .FirstOrDefault();

            var modaFormaOperacion = ftdata.GroupBy(r => r.forma_operacion.forma)
                .Select(grp => new {
                    Nombre = grp.Key,
                    Frecuencia = grp.Count()
                }).OrderByDescending(x => x.Frecuencia).Select(x => x.Nombre) // Selecciona solo el nombre
                .FirstOrDefault();

            iData.Add(new iGeneralMayoristaData
            {
                TotalImpVentas = TotalImpVentas,
                TotalCostVentas = TotalCostVentas,
                TotalUtilidades = TotalUtilidades,
                modaClienteNombre = modaClienteNombre,
                modaProductoServi = modaProductoServi,
                modaFormaOperacion = modaFormaOperacion,

            });

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        [HttpGet]
        public ContentResult MayoristaDistVCliente(string inicio, string fin, int productoID, int clienteID, int operacionID, int facturaID, int medioPagoID, string factura)
        {
            DateTime cvDateINI = Convert.ToDateTime(inicio);
            DateTime cvDateFIN = Convert.ToDateTime(fin);

            // Obtener todos los datos necesarios en una sola consulta
            var ftdata = db.negocio.Include(n => n.cliente).Include(n => n.forma_operacion).Include(n => n.medio_pago).Include(n => n.tipo_factura).Where(n => n.fecha >= cvDateINI & n.fecha <= cvDateFIN);

            if (productoID != 0) { ftdata = ftdata.Where(x => x.productoID == productoID); }
            if (clienteID != 0) { ftdata = ftdata.Where(x => x.clienteID == clienteID); }
            if (operacionID != 0) { ftdata = ftdata.Where(x => x.operacionID == operacionID); }
            if (facturaID != 0) { ftdata = ftdata.Where(x => x.facturaID == facturaID); }
            if (medioPagoID != 0) { ftdata = ftdata.Where(x => x.medioPagoID == medioPagoID); }
            if (factura != "") { ftdata = ftdata.Where(x => x.factura == factura); }
            
            var total_cantventas = ftdata.ToList().Count;

            var dist_vcliente = ftdata.GroupBy(r => r.cliente.nombre)
                .Select(rp => new {
                    grp = rp.Key,
                    Count = rp.Count(),
                    Cantidad = rp.Sum(x => x.cantidad),
                    Utilidad = rp.Sum(x => x.cantidad * (x.producto_servicio.precio - x.producto_servicio.costo))
                }).OrderBy(x => x.grp).ToList();

            return Content(JsonConvert.SerializeObject(dist_vcliente), "application/json");
        }

        [HttpGet]
        public ContentResult MayoristaDistVCanal(string inicio, string fin, int productoID, int clienteID, int operacionID, int facturaID, int medioPagoID, string factura)
        {
            DateTime cvDateINI = Convert.ToDateTime(inicio);
            DateTime cvDateFIN = Convert.ToDateTime(fin);

            // Obtener todos los datos necesarios en una sola consulta
            var ftdata = db.negocio.Include(n => n.cliente).Include(n => n.forma_operacion).Include(n => n.medio_pago).Include(n => n.tipo_factura).Where(n => n.fecha >= cvDateINI & n.fecha <= cvDateFIN);

            if (productoID != 0) { ftdata = ftdata.Where(x => x.productoID == productoID); }
            if (clienteID != 0) { ftdata = ftdata.Where(x => x.clienteID == clienteID); }
            if (operacionID != 0) { ftdata = ftdata.Where(x => x.operacionID == operacionID); }
            if (facturaID != 0) { ftdata = ftdata.Where(x => x.facturaID == facturaID); }
            if (medioPagoID != 0) { ftdata = ftdata.Where(x => x.medioPagoID == medioPagoID); }
            if (factura != "") { ftdata = ftdata.Where(x => x.factura == factura); }
            
            var total_cantventas = ftdata.ToList().Count;

            var dist_cventa = ftdata.GroupBy(r => r.forma_operacion.forma)
                            .Select(rp => new {
                                grp = rp.Key,
                                Count = rp.Count(),
                                Cantidad = rp.Sum(x => x.cantidad),
                                Utilidad = rp.Sum(x => x.cantidad * (x.producto_servicio.precio - x.producto_servicio.costo)),
                                Porcentaje = total_cantventas > 0 ? (rp.Count() / (double)total_cantventas) * 100 : 0
                            }).OrderBy(x => x.grp).ToList();

            return Content(JsonConvert.SerializeObject(dist_cventa), "application/json");
        }

        [HttpGet]
        public ContentResult MayoristaDistMedioPago(string inicio, string fin, int productoID, int clienteID, int operacionID, int facturaID, int medioPagoID, string factura)
        {
            DateTime cvDateINI = Convert.ToDateTime(inicio);
            DateTime cvDateFIN = Convert.ToDateTime(fin);

            // Obtener todos los datos necesarios en una sola consulta
            var ftdata = db.negocio.Include(n => n.cliente).Include(n => n.forma_operacion).Include(n => n.medio_pago).Include(n => n.tipo_factura).Where(n => n.fecha >= cvDateINI & n.fecha <= cvDateFIN);

            if (productoID != 0) { ftdata = ftdata.Where(x => x.productoID == productoID); }
            if (clienteID != 0) { ftdata = ftdata.Where(x => x.clienteID == clienteID); }
            if (operacionID != 0) { ftdata = ftdata.Where(x => x.operacionID == operacionID); }
            if (facturaID != 0) { ftdata = ftdata.Where(x => x.facturaID == facturaID); }
            if (medioPagoID != 0) { ftdata = ftdata.Where(x => x.medioPagoID == medioPagoID); }
            if (factura != "") { ftdata = ftdata.Where(x => x.factura == factura); }
           
            var total_cantventas = ftdata.ToList().Count;

            var dist_mpago = ftdata.GroupBy(r => r.medio_pago.medio)
               .Select(rp => new {
                   grp = rp.Key,
                   Porcentaje = total_cantventas > 0 ? (rp.Count() / (double)total_cantventas) * 100 : 0
               }).OrderBy(x => x.grp).ToList();

            return Content(JsonConvert.SerializeObject(dist_mpago), "application/json");
        }

        [HttpGet]
        public ContentResult MayoristaDistTipoCliente(string inicio, string fin, int productoID, int clienteID, int operacionID, int facturaID, int medioPagoID, string factura)
        {
            DateTime cvDateINI = Convert.ToDateTime(inicio);
            DateTime cvDateFIN = Convert.ToDateTime(fin);

            // Obtener todos los datos necesarios en una sola consulta
            var ftdata = db.negocio.Include(n => n.cliente).Include(n => n.forma_operacion).Include(n => n.medio_pago).Include(n => n.tipo_factura).Where(n => n.fecha >= cvDateINI & n.fecha <= cvDateFIN);

            if (productoID != 0) { ftdata = ftdata.Where(x => x.productoID == productoID); }
            if (clienteID != 0) { ftdata = ftdata.Where(x => x.clienteID == clienteID); }
            if (operacionID != 0) { ftdata = ftdata.Where(x => x.operacionID == operacionID); }
            if (facturaID != 0) { ftdata = ftdata.Where(x => x.facturaID == facturaID); }
            if (medioPagoID != 0) { ftdata = ftdata.Where(x => x.medioPagoID == medioPagoID); }
            if (factura != "") { ftdata = ftdata.Where(x => x.factura == factura); }
            
            var total_cantventas = ftdata.ToList().Count;

            var dist_tcliente = ftdata.GroupBy(r => r.cliente.tipo_cliente.tipo)
                .Select(rp => new {
                    grp = rp.Key,
                    Porcentaje = total_cantventas > 0 ? (rp.Count() / (double)total_cantventas) * 100 : 0
                }).OrderBy(x => x.grp).ToList();

            return Content(JsonConvert.SerializeObject(dist_tcliente), "application/json");
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