using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;
using System.Xml.Linq;
using vercom.Helpers;
using vercom.Interfaces;
using vercom.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;
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
            return View();
        }

        [HttpGet]
        public ContentResult Details(int id = 0, int tipo = 0, string inicio = "", string fin = "" , int pventa = 0, int categ = 0)
        {
        
          
            var cvDateIni = Convert.ToDateTime(inicio);
            if (tipo == 5) cvDateIni = cvDateIni.AddDays(-1);
        
            var cvDateFin = Convert.ToDateTime(fin);           
            int tipoPagoID = 0;

            var paramList = new[]
            {
                new SqlParameter("@FechaInicio", (object) cvDateIni ?? DBNull.Value),
                new SqlParameter("@FechaFin", (object) cvDateFin ?? DBNull.Value),
                new SqlParameter("@TipoOperacionID", tipo),
                new SqlParameter("@TipoPagoID", tipoPagoID),
                new SqlParameter("@PuntoVentaID", pventa),
                new SqlParameter("@CategoriaID", categ),
                new SqlParameter("@ProductoID", id)
            };

            try
            {
                db.Database.CommandTimeout = 120;
                var resultado = db.Database.SqlQuery<iOperacion>(
                    "EXEC sp_ListarOperacionesPorRangoFecha @FechaInicio, @FechaFin, @TipoOperacionID, @TipoPagoID, @PuntoVentaID, @CategoriaID, @ProductoID",
                    paramList
                    ).ToList();
                if (resultado == null)
                {
                    var mensaje = "Operación no encontrada";
                    return Content(JsonConvert.SerializeObject(mensaje), "application/json");                 
                }
                return Content(JsonConvert.SerializeObject(resultado), "application/json");
            } catch (Exception ex) {
                // Loguear el error si es necesario
                var mensaje = "Error interno del servidor";
                return Content(JsonConvert.SerializeObject(mensaje), "application/json");
            }
        }

        [RBAC]
        public ActionResult IPV()
        {           
            return View();
        }

        public ContentResult JsonIpvResult(string inicio, string fin, int pventa = 0, int categ = 0)
        {
          
            var cvDateIni = Convert.ToDateTime(inicio);
            var cvDateFin = Convert.ToDateTime(fin);
            // Generar datos desde SP con helper reutilizable
            var iData = IPVHelper.GenerarAnaliticoDesdeSP(cvDateIni, cvDateFin, pventa, categ);
            return Content(JsonConvert.SerializeObject(iData), "application/json");
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
        public JsonResult Create(operacion operacion, List<iProductoOperacion> productos)
        {
            try
            {
                var cvDateSal = operacion.fecha.Value.AddDays(-1);
                var userName = HttpContext.User.Identity.Name;

                var user = db.Users.FirstOrDefault(u => u.UserName == userName);
                if (user == null)
                    return Json(new { success = false, message = "Usuario no encontrado." });

                var rol = db.UserRoles.Where(u => u.Users.UserName == userName)
                                      .Select(u => u.Roles.RoleName)
                                      .FirstOrDefault();

                var notificacionesParaGuardar = new List<Notifications>();
                var operacionesParaGuardar = new List<operacion>();

                var productosDB = db.producto.ToDictionary(x => x.id);

                foreach (var item in productos)
                {
                    if (!productosDB.ContainsKey(item.ProductoID)) continue;

                    var producto = productosDB[item.ProductoID];
                    var categoriaID = producto.categoriaid;

                    var existeCierreCategoria = db.operacion.Any(r =>
                        r.punto_ventaid == operacion.punto_ventaid &&
                        r.fecha == operacion.fecha &&
                        r.tipo_operacionid == 5 &&
                        r.producto.categoriaid == categoriaID);

                    if (existeCierreCategoria)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"No se pueden agregar operaciones: ya existe cierre de categoría en la fecha {operacion.fecha.Value:d}."
                        });
                    }

                    var puntoVenta = db.punto_venta.FirstOrDefault(p => p.id == operacion.punto_ventaid);
                    if (puntoVenta == null) continue;

                    float? cantidad = ConvertToFloat(item.Cantidad?.ToString());
                    float? importe = ConvertToFloat(item.Importe?.ToString());

                    if (cantidad == null)
                    {
                        return Json(new { success = false, message = "La cantidad ingresada no es válida." });
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

                    var mensaje = $"Nueva operación para el producto {producto.nombre} - {producto.cod} en el punto de venta {puntoVenta.nombre}";

                    notificacionesParaGuardar.Add(new Notifications
                    {
                        UserId = user.UserID,
                        Message = mensaje,
                        CreatedAt = DateTime.Now,
                        Role = rol,
                        IsRead = false
                    });

                    if (saldo == null && operacion.tipo_operacionid != 5)
                    {
                        operacionesParaGuardar.Add(new operacion
                        {
                            cantidad = 0,
                            fecha = cvDateSal,
                            importe = 0,
                            productoid = item.ProductoID,
                            punto_ventaid = operacion.punto_ventaid,
                            tipo_operacionid = 5
                        });
                    }

                    db.trazas.Add(new trazas
                    {
                        fecha = DateTime.Now,
                        usuario = userName,
                        accion = "Registro de Operación",
                        descripcion = mensaje,
                        ip = Request.UserHostAddress,
                        modulo = "Operaciones"
                    });
                }

                db.operacion.AddRange(operacionesParaGuardar);
                db.SaveChanges();

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                foreach (var notificacion in notificacionesParaGuardar)
                {
                    hubContext.Clients.All.RecibirNotificacion(notificacion);
                }

                return Json(new { success = true, message = "Operaciones registradas correctamente.", redirectUrl = Url.Action("Index") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error interno: {ex.Message}" });
            }
        }

        private float? ConvertToFloat(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            input = input.Replace(",", ".");
            return (float?)Convert.ToDecimal(input, CultureInfo.InvariantCulture);
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
            var cvDateIni = DateTime.Parse(fecha);
            var cvDateSal = cvDateIni.AddDays(-1);

            // ⚡️ Extraer todas operaciones del día actual y saldo anterior en un solo paso
            var operacionesHoy = db.operacion
                .Where(o => o.punto_ventaid == pventa && o.fecha == cvDateIni && o.producto.categoriaid == categ)
                .ToList();

            var saldosPrevios = db.operacion
                .Where(o => o.punto_ventaid == pventa && o.fecha == cvDateSal && o.tipo_operacionid == 5 && o.producto.categoriaid == categ)
                .ToList();

            var iData = new List<iPVAnalitic>();
            var nuevosCierres = new List<operacion>();

            foreach (var saldo in saldosPrevios)
            {
                int idProd = (int) saldo.productoid;
                var producto = saldo.producto;

                double entradas = GetSum(operacionesHoy, 1, idProd);
                double ventas = GetSum(operacionesHoy, 2, idProd);
                double devoluciones = GetSum(operacionesHoy, 3, idProd);
                double mermas = GetSum(operacionesHoy, 4, idProd);

                var precio = (double) producto.precio;
                var totalEntrada = entradas * precio;
                var totalSalida = (ventas + devoluciones + mermas) * precio;

                var finalSaldo = saldo.cantidad + entradas - (ventas + devoluciones + mermas);
                var finalImporte = saldo.importe + totalEntrada - totalSalida;

                iData.Add(new iPVAnalitic
                {
                    id = producto.id,
                    cod = producto.cod,
                    producto = producto.nombre,
                    fecha = (DateTime) saldo.fecha,
                    unidad = producto.unidad.unidad1,
                    final_saldo = finalSaldo,
                    final_importe = finalImporte
                });

                // 👉 Evita crear duplicados de cierre
                bool yaExiste = operacionesHoy.Any(o => o.productoid == idProd && o.tipo_operacionid == 5);
                if (!yaExiste)
                {
                    nuevosCierres.Add(new operacion
                    {
                        fecha = cvDateIni,
                        cantidad = (float)finalSaldo,
                        importe = (float)finalImporte,
                        productoid = idProd,
                        punto_ventaid = pventa,
                        tipo_operacionid = 5
                    });
                }
            }

            // 🧠 Guardar solo una vez
            if (nuevosCierres.Any())
            {
                db.operacion.AddRange(nuevosCierres);
                db.SaveChanges();
            }

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        public ContentResult CierreJsonConsultResult(string fecha, int pventa, int categ)
        {
            var cvDateIni = Convert.ToDateTime(fecha);
            var cvDateFin = Convert.ToDateTime(fecha);

            var paramList = new[]
            {
                new SqlParameter("@PuntoVentaId", pventa),
                new SqlParameter("@Fecha", cvDateIni),
                new SqlParameter("@CategoriaId", categ)
            };

            db.Database.CommandTimeout = 120;

            var resultado = db.Database.SqlQuery<ResultadoSaldo>(
                "EXEC sp_ExisteSaldo @PuntoVentaId, @Fecha, @CategoriaId",
                paramList
            ).FirstOrDefault();

            bool existeSaldo = resultado?.ExisteSaldo ?? false;
            if (existeSaldo) {   var yaExiste = new List<iPVAnalitic>  {
            new iPVAnalitic
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
                importe_merma = 0
            }};

                return Content(JsonConvert.SerializeObject(yaExiste), "application/json");
            }

            // Generar datos desde SP con helper reutilizable
            var iData = IPVHelper.GenerarAnaliticoDesdeSP(cvDateIni, cvDateFin, pventa, categ);
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
            return View(operacion);
        }

        [HttpPost]
        public JsonResult Actualizar(int edt_edit, float? edt_cantidad)
        {
            var operacion = db.operacion.Where(o => o.id == edt_edit).SingleOrDefault();

            var existeCierreCategoria = db.operacion.Any(r =>
                r.punto_ventaid == operacion.punto_ventaid &&
                r.fecha == operacion.fecha &&
                r.tipo_operacionid == 5 &&
                r.producto.categoriaid == operacion.producto.categoriaid);

            if (existeCierreCategoria)
            {
                return Json(new
                {
                    success = false,
                    message = $"No se puedo actualizar operacion: ya existe cierre en la fecha {operacion.fecha.Value:d}."
                });
            }

            var precio = operacion.producto.precio;
            operacion.cantidad = edt_cantidad;
            operacion.importe = (float?)(edt_cantidad * precio);        

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
            return Json(new { success = true });
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

        public ContentResult ObtenerOperacion(int id = 0)
        {
            var paramList = new[] {            
                new SqlParameter("@OperacionId", id)     
            };

            db.Database.CommandTimeout = 120;
            var iData = db.Database.SqlQuery<iOperacion>(
                "EXEC sp_ObtenerOperacion @OperacionId",paramList
            ).ToList();

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        public ContentResult listFilterOperacion(string fecha = "", int tperacion = 0, int tpago = 0, int pventa = 0, int categ = 0, int prod = 0)
        {
            var fechaParam = string.IsNullOrEmpty(fecha)
                ? new SqlParameter("@Fecha", DBNull.Value)
                : new SqlParameter("@Fecha", Convert.ToDateTime(fecha));
            
            var paramList = new[] {
                fechaParam,
                new SqlParameter("@TipoOperacionID", tperacion),
                new SqlParameter("@TipoPagoID", tpago),
                new SqlParameter("@PuntoVentaID", pventa),
                new SqlParameter("@CategoriaID", categ),
                new SqlParameter("@ProductoID", prod)
            };
            db.Database.CommandTimeout = 120;
            var iData = db.Database.SqlQuery<iOperacion>(
                "EXEC sp_ListarOperacionesFiltradas @Fecha, @TipoOperacionID, @TipoPagoID, @PuntoVentaID, @CategoriaID, @ProductoID",
                paramList
            ).ToList();

            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
    public class ResultadoSaldo
    {
        public bool ExisteSaldo { get; set; }
    }
}