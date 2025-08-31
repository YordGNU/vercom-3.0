using Microsoft.AspNet.SignalR;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using vercom.Models;

public class StockJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        var db = new VERCOMEntities();

        var ultimaOperacion = db.operacion.Where(p=> p.tipo_operacionid == 5)
            .OrderByDescending(p => p.fecha)// Ordena por fecha, de la más reciente a la más antigua
            .Select(p=>p.fecha)
            .FirstOrDefault(); // Obtiene la primera operación de la lista

        var cvDate = ultimaOperacion.Value.AddDays(-1);

        var productosBajoStock = db.operacion
            .Where(p => p.cantidad <= 10 && p.tipo_operacionid == 5 && p.fecha <= ultimaOperacion && p.fecha >= cvDate)
            .Select(p => new { p.producto.nombre, p.cantidad, p.producto.unidad.unidad1, Punto = p.punto_venta.nombre})
            .ToList();

        foreach (var producto in productosBajoStock)
        {
            string mensaje = $"{producto.nombre} tiene bajo stock ({producto.cantidad} {producto.unidad1} ), en {producto.Punto}"; 
            var existeNotificacion = db.Notifications.Where(m => m.Message == mensaje).FirstOrDefault();          
            if (existeNotificacion != null) 
            {
                // Guardar la notificación en la base de datos
                existeNotificacion.Message = mensaje;    
                existeNotificacion.CreatedAt = DateTime.Now;                    
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.Group("Comercial").RecibirNotificacion(mensaje);
            }else {
                // Guardar la notificación en la base de datos
                var notificacion = new Notifications
                {
                    UserId = 1,
                    Message = mensaje,
                    CreatedAt = DateTime.Now, // Mejor usar UTC
                    Role = "Comercial",
                    IsRead = false
                };
                db.Notifications.Add(notificacion);
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.Group("Comercial").RecibirNotificacion(mensaje);
            }
        }
        db.SaveChanges();
        return Task.CompletedTask;
    }
}