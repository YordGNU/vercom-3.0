using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using vercom.Interfaces;
using vercom.Models;
using EntityState = System.Data.Entity.EntityState;

namespace vercom.Controllers
{

    public class ParcialController : Controller
    {
        private VERCOMEntities db = new VERCOMEntities();       

        public ActionResult _listapuntos()
        {
            var consulta = from r in db.punto_venta orderby r.nombre ascending select r;
            return PartialView("_listapuntos", consulta.ToList());
        }

        public ActionResult _listapuntosName()
        {
            var consulta = from r in db.punto_venta orderby r.nombre ascending select r;
            return PartialView("_listapuntosName", consulta.ToList());
        }


        public ActionResult _listunidades()
        {
            var consulta = from r in db.unidad orderby r.unidad1 ascending select r;
            return PartialView("_listunidades", consulta.ToList());
        }
        public ActionResult _listunidadesName()
        {
            var consulta = from r in db.unidad orderby r.unidad1 ascending select r;
            return PartialView("_listunidadesName", consulta.ToList());
        }

        public ActionResult _listoperaciontipo()
        {
            var consulta = from r in db.tipo_operacion orderby r.id ascending select r;
            return PartialView("_listoperaciontipo", consulta.ToList());
        }

        public ActionResult _listoperaciontipoName()
        {
            var consulta = from r in db.tipo_operacion orderby r.id ascending select r;
            return PartialView("_listoperaciontipoName", consulta.ToList());
        }

        public ActionResult _listtipopago()
        {
            var consulta = from r in db.tipo_pago orderby r.tipo ascending select r;
            return PartialView("", consulta.ToList());
        }

        public ActionResult _listcategorias()
        {
            var consulta = from r in db.categoria orderby r.clave ascending select r;
            return PartialView("_listcategorias", consulta.ToList());
        }
        public ActionResult _listcategoriasName()
        {
            var consulta = from r in db.categoria orderby r.clave ascending select r;
            return PartialView("_listcategoriasName", consulta.ToList());
        }

        public ActionResult _listtproductos()
        {
            var consulta = from r in db.producto orderby r.nombre ascending select r;
            return PartialView("_listtproductos", consulta.ToList());
        }

        public ActionResult _listtproductospunto(int idpunto, DateTime fecha)
        {
            fecha = fecha.AddDays(-1);
            var consulta = (from r in db.operacion where r.punto_ventaid == idpunto && r.tipo_operacionid == 5 && r.tipo_operacionid == 1 && r.fecha == fecha orderby r.producto.nombre ascending select r.producto);
            return PartialView("_listtproductos", consulta.ToList());
        }

        public ActionResult _operacionsTable()
        {
            var consulta = from r in db.operacion orderby r.fecha ascending select r;
            return PartialView("_operacionsTable", consulta.ToList());
        }

        public ActionResult _listareas()
        {
            var consulta = from r in db.area orderby r.nombre ascending select r;
            return PartialView("_listareas", consulta.ToList());
        }

        public ActionResult _listpresupuestos()
        {
            var fecha = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            var fechafilter = Convert.ToDateTime(fecha);
            var consulta = (from r in db.presupuesto where (r.fecha == fechafilter) select r);
            return PartialView("_listpresupuestos", consulta.ToList());
        }

        public ActionResult _listclientes()
        {
            var consulta = from r in db.cliente orderby r.nombre ascending select r;
            return PartialView("_listclientes", consulta.ToList()); // Ajusta el nombre de la vista parcial
        }

        public ActionResult _listtiposclientes()
        {
            var consulta = from r in db.tipo_cliente orderby r.tipo ascending select r;
            return PartialView("_listtiposclientes", consulta.ToList()); // Ajusta el nombre de la vista parcial
        }

        public ActionResult _listproductos_servicios()
        {
            var consulta = from r in db.producto_servicio orderby r.nombre ascending select r;
            return PartialView("_listproductos_servicios", consulta.ToList()); // Ajusta el nombre de la vista parcial
        }

        public ActionResult _listtipofactura()
        {
            var consulta = from r in db.tipo_factura orderby r.tipo ascending select r;
            return PartialView("_listtipofactura", consulta.ToList()); // Ajusta el nombre de la vista parcial
        }

        public ActionResult _listmediopago()
        {
            var consulta = from r in db.medio_pago orderby r.medio ascending select r;
            return PartialView("_listmediopago", consulta.ToList()); // Ajusta el nombre de la vista parcial
        }

        public ActionResult _listformaoperacion()
        {
            var consulta = from r in db.forma_operacion orderby r.forma ascending select r;
            return PartialView("_listformaoperacion", consulta.ToList()); // Ajusta el nombre de la vista parcial
        }
        public ContentResult _listpresupuestosFecha(string fecha)
        {
            List<iFlujoResumen> iData = new List<iFlujoResumen>();
            var cvFecha = Convert.ToDateTime(fecha);
            var cvFechaFilter = cvFecha.AddDays(-1);
            var consulta = (from d in db.presupuesto
                            where (d.fecha == cvFechaFilter)
                            group d by d.areaid into rp
                            select new
                            {
                                grp = rp.Key,
                                areaID = rp.Select(x => x.areaid).FirstOrDefault(),
                                area = rp.Select(x => x.area.nombre).FirstOrDefault(),
                            }).OrderBy(x => x.grp).ToList();

            foreach (var item in consulta)
            {
                iFlujoResumen iVal = new iFlujoResumen
                {
                    areaID = item.areaID,
                    area = item.area,
                };
                iData.Add(iVal);
            }
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        public ContentResult _productosXpunto(int id, int tipo, string fecha)
        {
            List<iProductosXpunto> iData = new List<iProductosXpunto>();
            var cvFecha = Convert.ToDateTime(fecha);
            var cvFechaSaldo = cvFecha.AddDays(-1);
            var saldo = (from r in db.operacion where (r.fecha == cvFechaSaldo) && r.punto_ventaid == id && r.tipo_operacionid == 5 select r).ToList();
            if (tipo == 5 || tipo == 1)
            {
                var query = (from r in db.producto select r).ToList();

                foreach (var item in query)
                {
                    iProductosXpunto iVal = new iProductosXpunto
                    {
                        id = item.id,
                        cod = item.cod,
                        nombre = item.nombre,    
                        precio = item.precio,
                        cant_saldo = saldo.Where(s=>s.productoid == item.id).Select(s=>s.cantidad).FirstOrDefault(),
                    };
                    iData.Add(iVal);
                }

            }
            else
            {              
                foreach (var item in saldo)
                {
                    iProductosXpunto iVal = new iProductosXpunto
                    {
                        id = item.producto.id,
                        cod = item.producto.cod,
                        nombre = item.producto.nombre,
                        precio = item.producto.precio,
                        cant_saldo = item.cantidad
                    };
                    iData.Add(iVal);
                }

            }
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        public ContentResult _productosXpuntoFilCategoria(int id, int tipo, string fecha, int categ)
        {
            List<iProductosXpunto> iData = new List<iProductosXpunto>();
            var cvFecha = Convert.ToDateTime(fecha);
            var cvFechaSaldo = cvFecha.AddDays(-1);
            var dataSaldo = db.operacion.Where(s => s.fecha == cvFechaSaldo && s.producto.categoriaid == categ && s.punto_ventaid == id).ToList();
            // Obtener el saldo anterior
            var saldo = db.operacion.ToList()
                .Where(r => r.tipo_operacionid == 5)
                .GroupBy(r => r.productoid)
                .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(r => r.cantidad) })
                .ToDictionary(s => s.ProductoId, s => s.Cantidad);

            // Obtener las entradas desde la fecha dada
            var entradas = dataSaldo
                .Where(r => r.fecha >= cvFecha && r.tipo_operacionid == 1)
                .GroupBy(r => r.productoid)
                .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(r => r.cantidad) })
                .ToDictionary(e => e.ProductoId, e => e.Cantidad);

            if (tipo == 5 || tipo == 1)
            {
                var productos = db.producto.ToList();
                foreach (var item in productos)
                {
                    iProductosXpunto iVal = new iProductosXpunto
                    {
                        id = item.id,
                        cod = item.cod,
                        nombre = item.nombre,
                        precio = item.precio,
                        cant_saldo = (saldo.ContainsKey(item.id) ? saldo[item.id] : 0) +
                                     (entradas.ContainsKey(item.id) ? entradas[item.id] : 0),
                    };
                    iData.Add(iVal);
                }
            }
            else
            {
                var productosFiltrados = saldo.Keys.ToList()
                    .Where(pid => saldo[pid] > 0 || entradas.ContainsKey(pid) && db.producto.First(p => p.id == pid).categoriaid == categ)
                    .Select(pid => new iProductosXpunto
                    {
                        id = (int) pid,
                        cod = db.producto.First(p => p.id == pid).cod,
                        nombre = db.producto.First(p => p.id == pid).nombre,
                        precio = db.producto.First(p => p.id == pid).precio,
                        cant_saldo = saldo[pid] + (entradas.ContainsKey(pid) ? entradas[pid] : 0),
                    }).ToList();
                iData.AddRange(productosFiltrados);
            }
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        public ContentResult _productosXprecio(int id, int idprod, string fecha)
        {
            List<producto> iData = new List<producto>();
            var cvFecha = Convert.ToDateTime(fecha);
            var cvFechaSaldo = cvFecha.AddDays(-1);
            var saldo = (from r in db.operacion where r.fecha == cvFechaSaldo && r.punto_ventaid == id && r.tipo_operacionid == 5 && r.productoid == idprod select r);
            var operacion = (from r in db.operacion where r.fecha == cvFecha && r.punto_ventaid == id && r.tipo_operacionid != 5 && r.productoid == idprod select r);
            double? finalSaldo = 0;
            double? precio_venta = db.producto.Where(p => p.id == idprod).Select(p => p.precio).FirstOrDefault();
            float? cantidad_entrada = 0;
            float? cantidad_venta = 0;
            float? cantidad_devolucion = 0;
            float? cantidad_merma = 0;

            foreach (var item in saldo)
            {
                var auxCe = (from r in operacion where r.tipo_operacionid == 1 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCe != null)
                {
                    cantidad_entrada = auxCe;
                }


                var auxCv = (from r in operacion where r.tipo_operacionid == 2 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCv != null)
                {
                    cantidad_venta = auxCv;
                }

                var auxCd = (from r in operacion where r.tipo_operacionid == 3 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCd != null)
                {
                    cantidad_devolucion = auxCd;
                }

                var auxCm = (from r in operacion where r.tipo_operacionid == 4 && r.productoid == item.productoid select r.cantidad).Sum();
                if (auxCm != null)
                {
                    cantidad_merma = auxCm;
                }

                finalSaldo += item.cantidad + cantidad_entrada - (cantidad_venta + cantidad_devolucion + cantidad_merma);
            }

            finalSaldo = Convert.ToDouble(String.Format("{0:#,##0.00}", finalSaldo));
            producto iVal = new producto
            {
                precio = precio_venta,
                costo = finalSaldo,
            };

            iData.Add(iVal);
            return Content(JsonConvert.SerializeObject(iData), "application/json");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}