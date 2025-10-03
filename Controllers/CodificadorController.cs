using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vercom.Interfaces;
using vercom.Models;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace vercom.Controllers
{
    [Authorize]
    public class CodificadorController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();

        [RBAC]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCategorias()
        {
            try
            {
                var categorias = db.categoria.ToList();             
                List<CategoriasDTO> iData = new List<CategoriasDTO>();
                foreach (var item in categorias) {
                    iData.Add(new CategoriasDTO {
                           ID = item.id,
                           Clave =item.clave                    
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

        [HttpGet]
        public JsonResult GetSubCategorias()
        {
            try
            {
                var areas = db.area.ToList();
                List<SubCategoriasDTO> iData = new List<SubCategoriasDTO>();
                foreach (var item in areas)
                {
                    iData.Add(new SubCategoriasDTO
                    {
                        ID = item.id,
                        Clave = item.nombre
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

        [HttpGet]
        public JsonResult GetUnidades()
        {
            try
            {
                var unidad = db.unidad.ToList();
                List<UnidadesDTO> iData = new List<UnidadesDTO>();
                foreach (var item in unidad)
                {
                    iData.Add(new UnidadesDTO
                    {
                        ID = item.id,
                        Clave = item.unidad1
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

        [HttpGet]
        public JsonResult GetTOperaciones()
        {
            try
            {
                var toperacion = db.tipo_operacion.ToList();
                List<TOperacionesDTO> iData = new List<TOperacionesDTO>();
                foreach (var item in toperacion)
                {
                    iData.Add(new TOperacionesDTO
                    {
                        ID = item.id,
                        Clave = item.tipo
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

        [HttpGet]
        public JsonResult GetTPago()
        {
            try
            {
                var tpago = db.tipo_pago.ToList();
                List<TPagosDTO> iData = new List<TPagosDTO>();
                foreach (var item in tpago)
                {
                    iData.Add(new TPagosDTO
                    {
                        ID = item.id,
                        Clave = item.tipo
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
    }
}