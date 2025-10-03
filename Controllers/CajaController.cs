using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using vercom.Interfaces;
using vercom.Models;

namespace vercom.Controllers
{
    public class CajaController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();
        // GET: Caja
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cierre()
        {
            return View();
        }

        public ActionResult Historial()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerResumenCierre(int cajaId)
        {
              // Fecha de hoy (solo parte de fecha)
            var hoy = DateTime.Now;        
            var cierreAnterior = db.CierreCaja.Where(c => c.CajaID == cajaId && c.FechaCierre < hoy)
                .OrderByDescending(c => c.FechaCierre)
                .FirstOrDefault();

            var fechaInicio = cierreAnterior != null ? cierreAnterior.FechaCierre : DateTime.MinValue;

            var movimientos = db.MovimientoCaja
                .Where(m => m.SubMayor.CajaID == cajaId &&
                 m.Fecha > fechaInicio && m.Fecha <= hoy)
                .ToList();

            var cantidad = movimientos.Count();


            var entradas = movimientos.Where(m => m.TipoMovimiento == "Entrada").Sum(m => m.Monto);
            var salidas = movimientos.Where(m => m.TipoMovimiento == "Salida").Sum(m => m.Monto);

            // Saldo actual de la caja principal
            var saldoCajaPrincipal = db.CajaPrincipal
                    .Where(c => c.CajaID == cajaId)
                    .Select(c => (decimal?)c.SaldoActual)
                    .FirstOrDefault() ?? 0m;
            
            var saldoCierre = db.CierreCaja
                .Where(m => m.CajaID == cajaId &&
                m.FechaCierre <= hoy &&
                m.Estado == "Confirmado")
                .OrderByDescending(m => m.FechaCierre)
                .FirstOrDefault();

            decimal saldoInicial = saldoCierre != null ? saldoCierre.SaldoFinal : 0;

            // Construye el objeto de resumen

            return Json(new
            {
                exito = true,
                SaldoInicial = saldoInicial,
                CantidadMovimientos = cantidad,
                TotalEntradas = entradas,
                TotalSalidas = salidas,
                SaldoTotal = saldoCajaPrincipal
            }, JsonRequestBehavior.AllowGet);

        }    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RegistrarMovimiento(MovimientoCaja model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { exito = false, mensaje = "Datos incompletos o inválidos." });
            }
            
            try
                {

                var cajaID = db.SubMayor.Find(model.SubMayorID).CajaID;

                // 1) Consulta si existe un cierre confirmado en la fecha y caja del movimiento
                // 1) Validar si la caja está cerrada para la fecha del movimiento
                var validation = db.Database
                    .SqlQuery<iCajaCierreValidacion>(
                        "EXEC sp_ValidarCajaCerrada @p0, @p1",
                        cajaID,              // id de la caja
                        DateTime.Now             // fecha/hora del movimiento
                    )
                    .FirstOrDefault();

                if (validation != null && validation.EstaCerrada == 1)
                {
                    // Ya cerrada: abortar e informar al usuario
                    var f = validation.FechaHoraCierre?.ToString("yyyy-MM-dd HH:mm");
                    return Json(new
                    {
                        exito = false,
                        mensaje = $"No se puede registrar el movimiento. " +
                                  $"La caja fue cerrada el {f}."
                    });
                }


                // Ejecutar procedimiento de validación
                var resultado = db.Database.SqlQuery<string>(
                    "EXEC sp_ValidarMovimientoCaja @p0, @p1, @p2, @p3",
                    model.SubMayorID,
                    model.TipoMovimiento,
                    model.Monto,
                    DateTime.Now
                    ).FirstOrDefault();

                if (resultado != "OK")
                {
                    return Json(new { exito = false, mensaje = resultado });
                }

                // Si pasó la validación, registrar el movimiento
                model.Fecha = DateTime.Now;
                model.Usuario = User.Identity.Name;
                db.MovimientoCaja.Add(model);
                db.SaveChanges(); // Dispara triggers: saldos, auditoría, alertas
                                 
                return Json(new
                    {
                        exito = true,
                        mensaje = "Movimiento registrado correctamente.",
                        redirectUrl = Url.Action("Index", "Caja")
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { exito = false, mensaje = "Error interno: " + ex.Message });
                }           
        }

        [HttpGet]
        public JsonResult ListarMovimientos(DateTime? filtroDesde, DateTime? filtroHasta, int? filtroSubMayor = 0, string filtroTipo = "" )
        {
            
            var iData = new List<iMovimiento>();
            var lista = db.MovimientoCaja
                .Include("SubMayor")
                .OrderByDescending(m => m.Fecha)
                    .Where(m => m.Fecha >= filtroDesde && m.Fecha <= filtroHasta)
                    .Select(m => new {
                        m.MovimientoID,
                        m.Fecha,          
                        SubMayorID = m.SubMayorID,
                        SubMayor = m.SubMayor.Nombre,
                        m.TipoMovimiento,
                        m.Monto,
                        m.Concepto,
                        m.ReferenciaExterna,
                        m.MetodoPago,
                        m.Usuario
                    }).ToList();

            if(filtroSubMayor != 0)          
               lista =  lista.Where(m => m.SubMayorID == filtroSubMayor).ToList();

            if (filtroTipo != "")
               lista =  lista.Where(m => m.TipoMovimiento == filtroTipo).ToList();
            

                foreach (var item in lista)
                {
                    iData.Add(new iMovimiento
                    {
                       MovimientoID = item.MovimientoID,
                       Fecha = item.Fecha,
                       SubMayor = item.SubMayor,
                       TipoMovimiento = item.TipoMovimiento,
                       Monto = item.Monto,
                       Concepto = item.Concepto,
                       ReferenciaExterna = item.ReferenciaExterna,
                       MetodoPago = item.MetodoPago,
                       Usuario = item.Usuario,                     
                    });

                }
                return Json(iData, JsonRequestBehavior.AllowGet);         
        }

        [HttpGet]
        public JsonResult ResumenFiltrado(DateTime? fechaDesde, DateTime? fechaHasta, int? subMayorId, string tipo)
        {

            var cajaId = 1;
            if (subMayorId != null) cajaId = db.SubMayor.Find(subMayorId).CajaID;
            var movimientos = db.MovimientoCaja
            .Where(m => m.SubMayor.CajaID == cajaId &&
                        m.Fecha >= fechaDesde &&
                        m.Fecha <= fechaHasta)
            .ToList();

            var cantidad = movimientos.Count;

            var saldoCierre = db.CierreCaja
                .Where(m => m.CajaID == cajaId &&
                            m.FechaCierre <= fechaHasta &&
                            m.Estado == "Confirmado")
                .OrderByDescending(m => m.FechaCierre)
                .FirstOrDefault();

            decimal saldoInicial = saldoCierre != null ? saldoCierre.SaldoFinal : 0;

            var entradas = movimientos.Where(m => m.TipoMovimiento == "Entrada").Sum(m => m.Monto);
            var salidas = movimientos.Where(m => m.TipoMovimiento == "Salida").Sum(m => m.Monto);
            var saldoCajaPrincipal = db.CajaPrincipal
                .Where(c => c.CajaID == cajaId)
                .Select(c => (decimal?)c.SaldoActual)
                .FirstOrDefault() ?? 0;

            return Json(new
            {
                exito = true,
                SaldoInicial = saldoInicial,
                CantidadMovimientos = cantidad,
                TotalEntradas = entradas,
                TotalSalidas = salidas,
                SaldoTotal = saldoCajaPrincipal
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]  
        public JsonResult ListaResumenSubMayores(int cajaId)
        {          
                try
                {        
                var resumen = db.Database
                        .SqlQuery<iSubMayoresResumen>(
                            "EXEC sp_ObtenerResumenSubMayoresPorCaja @p0", cajaId
                        )
                        .ToList();

                    return Json(resumen, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        exito = false,
                        mensaje = "Error al obtener el resumen de sub-mayores: " + ex.Message
                    }, JsonRequestBehavior.AllowGet);
                }            
        }

        [HttpGet]
        public JsonResult ObtenerHistorialCierres(int? cajaId, DateTime? desde, DateTime? hasta)
        {
            var iData = new List<iCierreCajaResumen>();
            var cierres = db.CierreCaja.Where(c => c.CajaID == cajaId && c.FechaCierre >= desde && c.FechaCierre <= hasta).OrderByDescending(c=>c.FechaCierre).ToList();
            foreach (var c in cierres)
            {
                iData.Add(new iCierreCajaResumen
                {
                    CierreID = c.CierreID,
                    FechaCierre = c.FechaCierre,
                    Caja = c.CajaPrincipal.Nombre,
                    TotalIngresos = c.TotalIngresos,
                    TotalEgresos = c.TotalEgresos,
                    SaldoFinal = c.SaldoFinal,
                    Usuario = c.Usuario,
                    Observaciones = c.Observaciones,
                    Estado = c.Estado

                });

            }
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerDetalleCierre(int id)
        {
            var cierreActual = db.CierreCaja.Find(id);
            if (cierreActual == null)
                return Json(new { error = true, mensaje = "Cierre no encontrado." }, JsonRequestBehavior.AllowGet);

            // Buscar el cierre anterior
            var fechaInicio = db.CierreCaja
                .Where(c => c.CajaID == cierreActual.CajaID &&
                            c.Estado == "Confirmado" &&
                            c.FechaCierre < cierreActual.FechaCierre)
                .OrderByDescending(c => c.FechaCierre)
                .Select(c => c.FechaCierre)
                .FirstOrDefault();

            if (fechaInicio == default(DateTime))
                fechaInicio = cierreActual.FechaCierre.AddHours(-8);

            var CierreSubMayores = db.CierreSubMayor
                .Where(c => c.CierreID == id)
                .ToList();

            var iData = new List<iCierreSubMayorResumen>();

            foreach (var item in CierreSubMayores)
            {
                var movimientos = db.MovimientoCaja
                    .Where(m => m.SubMayorID == item.SubMayorID &&
                                m.Fecha > fechaInicio &&
                                m.Fecha <= cierreActual.FechaCierre)
                    .OrderBy(m => m.Fecha)
                    .Select(m => new MovimientoDTO
                    {
                        Fecha = m.Fecha,
                        TipoMovimiento = m.TipoMovimiento,
                        Monto = m.Monto,              
                        Concepto = m.Concepto,
                        Referencia = m.ReferenciaExterna
                    }).ToList();

                var entradas = movimientos.Where(m => m.TipoMovimiento == "Entrada").ToList();
                var salidas = movimientos.Where(m => m.TipoMovimiento == "Salida").ToList();

                iData.Add(new iCierreSubMayorResumen
                {
                    Id = item.ID,
                    FechaRegistro = (DateTime)item.FechaRegistro,
                    CierreID = item.CierreID,
                    SubMayorID = item.SubMayorID,
                    SubMayorNombre = item.SubMayor.Nombre,
                    SaldoFinal = item.SaldoFinal,
                    Entradas = entradas,
                    Salidas = salidas
                });
            }

            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObtenerMovimientoPorId(int id)
        {
            var m = db.MovimientoCaja.Find(id);
            if (m == null) return Json(new { exito = false, mensaje = "Movimiento no encontrado." }, JsonRequestBehavior.AllowGet);
            var iData = new List<iMovimiento>();
            iData.Add(new iMovimiento
            {
                MovimientoID = m.MovimientoID,
                Fecha = m.Fecha,
                SubMayor = m.SubMayor.Nombre,
                SubMayorID = m.SubMayorID,
                SaldoSubMayor = m.SubMayor.Saldo,
                TipoMovimiento = m.TipoMovimiento,
                Monto = m.Monto,
                Concepto = m.Concepto,
                ReferenciaExterna = m.ReferenciaExterna,
                MetodoPago = m.MetodoPago,
                Usuario = m.Usuario
            });     
            
            return Json(new { exito = true, iData }, JsonRequestBehavior.AllowGet);           
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditarMovimiento(int Edit_MovimientoID, string Edit_MetodoPago, decimal Edit_Monto, string Edit_Concepto, string Edit_ReferenciaExterna)
        {
            try
            {

                var movimiento = db.MovimientoCaja.Find(Edit_MovimientoID);
                if (movimiento == null)
                    return Json(new { success = false, message = "Movimiento no encontrado." });

                // Validaciones básicas
                if (Edit_Monto <= 0)
                    return Json(new { success = false, message = "El monto debe ser mayor que cero." });

                var cajaID = movimiento.SubMayor.CajaID;

                // 1) Consulta si existe un cierre confirmado en la fecha y caja del movimiento
                // 1) Validar si la caja está cerrada para la fecha del movimiento
                var validation = db.Database
                    .SqlQuery<iCajaCierreValidacion>(
                        "EXEC sp_ValidarCajaCerrada @p0, @p1",
                        cajaID,              // id de la caja
                        movimiento.Fecha       // fecha/hora del movimiento
                    )
                    .FirstOrDefault();

                if (validation != null && validation.EstaCerrada == 1)
                {
                    // Ya cerrada: abortar e informar al usuario
                    var f = validation.FechaHoraCierre?.ToString("yyyy-MM-dd HH:mm");
                    return Json(new
                    {
                        exito = false,
                        mensaje = $"No se puede editar. La caja ya fue cerrada el {validation.FechaHoraCierre}."
                    });
                }

                // Actualización de campos              
                movimiento.Monto = Edit_Monto;
                movimiento.MetodoPago = Edit_MetodoPago;        
                movimiento.Concepto = Edit_Concepto;        
                movimiento.ReferenciaExterna = Edit_ReferenciaExterna;        
                movimiento.Usuario = User.Identity.Name; // o el usuario actual

                db.SaveChanges();

                return Json(new { exito = true, mensaje = "Movimiento editado correctamente." });
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
            var movimientos = db.MovimientoCaja.Where(o => ids.Contains(o.MovimientoID)).ToList();
            foreach(var item in movimientos)
            {
                var validation = db.Database
                        .SqlQuery<iCajaCierreValidacion>(
                            "EXEC sp_ValidarCajaCerrada @p0, @p1",
                            item.SubMayor.CajaID,              // id de la caja
                            item.Fecha       // fecha/hora del movimiento
                        )
                        .FirstOrDefault();

                if (validation != null && validation.EstaCerrada == 1)
                {
                    // Ya cerrada: abortar e informar al usuario
                    var f = validation.FechaHoraCierre?.ToString("yyyy-MM-dd HH:mm");
                    return Json(new
                    {
                        exito = false,
                        mensaje = $"No se puede eliminar. La caja ya fue cerrada el {validation.FechaHoraCierre}."
                    });
                }

            }
            db.MovimientoCaja.RemoveRange(movimientos);
            db.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public JsonResult CierreDiario(int cajaId, string usuario, string observaciones)
        {
          
                try
                {
                    var resumen = db.Database.SqlQuery<iCierreCajaResumen>(
                        "EXEC sp_CierreDiarioCaja @p0, @p1, @p2",
                        cajaId, usuario, observaciones
                    ).FirstOrDefault();

                    return Json(new { exito = true, resumen });
                }
                catch (Exception ex)
                {
                    return Json(new { exito = false, mensaje = "Error en el cierre: " + ex.Message });
                }
        }
        
    }
}