using Microsoft.AspNet.SignalR;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using vercom.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;

public class StockJob : IJob
{
   
    public void Execute(IJobExecutionContext context)
    {
        using (var db = new VERCOMEntities())
        {
            var usuariosConRol = db.UserRoles.Where(u => u.Roles.IsSysAdmin == true || u.Roles.RoleName == "Comercial")
                .Select(u => u.Users.UserName)// Este debe coincidir con User.Identity.Name
                .ToList();

            var ultimaOperacion = db.operacion.Where(p => p.tipo_operacionid == 5)
                .OrderByDescending(p => p.fecha)// Ordena por fecha, de la más reciente a la más antigua
                .Select(p => p.fecha)
                .FirstOrDefault(); // Obtiene la primera operación de la lista

            var cvDate = ultimaOperacion.Value.AddDays(-1);

            var productosBajoStock = db.operacion
                .Where(p => p.cantidad <= 10 && p.tipo_operacionid == 5 && p.fecha <= ultimaOperacion && p.fecha >= cvDate)
                .Select(p => new { p.producto.nombre, p.cantidad, p.producto.unidad.unidad1, Punto = p.punto_venta.nombre })
                .ToList();

            var agrupadas = productosBajoStock.GroupBy(p => p.nombre)
                .Select(g => new {
                    Producto = g.Key,
                    Total = g.Sum(x => x.cantidad),
                    Unidad = g.Select(x => x.unidad1).FirstOrDefault(),
                    Puntos = g.Select(x => x.Punto).Distinct()
                });

            var hub = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();

            foreach (var usuario in usuariosConRol)
            {
                foreach (var item in agrupadas)
                {
                    var puntos = string.Join(", ", item.Puntos);
                    var mensaje = $"{item.Producto} ({item.Total} {item.Unidad.ToString()}) en {puntos}.";
                    hub.Clients.User(usuario).recibirNotificacionStock("Stock bajo", mensaje, "warning");
                }
            }
        }
    }

    Task IJob.Execute(IJobExecutionContext context)
    {    
        throw new NotImplementedException();
    }
}