using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using vercom.Models;
using vercom.Hubs;

namespace vercom.Services
{
    public class StockService
    {
        private VERCOMEntities db = new VERCOMEntities();

        public void ComprobarStock()
        {
            var productosBajoStock = db.operacion
                .Where(p => p.cantidad <= 10 && p.tipo_operacionid == 5)
                .Select(p => new { p.producto.nombre, p.cantidad, Punto = p.punto_venta.nombre })
                .ToList();

            if (!productosBajoStock.Any()) return; // No ejecutar si no hay productos con stock bajo

            // Obtener usuario logueado y su rol en una sola consulta
            var usuario = db.Users
                .Where(u => u.UserName == System.Web.HttpContext.Current.User.Identity.Name)
                .Select(u => new
                {
                    u.UserID,
                    Rol = u.UserRoles.Select(r => r.Roles.RoleName).FirstOrDefault()
                })
                .FirstOrDefault();

            if (usuario == null) return; // Si el usuario no está autenticado, salir

            foreach (var producto in productosBajoStock)
            {
                string mensaje = $"El producto {producto.nombre} tiene bajo stock ({producto.cantidad} unidades), en el punto de venta {producto.Punto}.";
                var existeNotificacion = db.Notifications.Any(m => m.Message == mensaje);
                if (!existeNotificacion)
                {
                    // Guardar la notificación en la base de datos
                    var notificacion = new Notifications
                    {
                        UserId = usuario.UserID,
                        Message = mensaje,
                        CreatedAt = DateTime.Now, // Mejor usar UTC
                        Role = usuario.Rol,
                        IsRead = false
                    };

                    db.Notifications.Add(notificacion);
                    // Enviar notificación en tiempo real con SignalR
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                    hubContext.Clients.Group(usuario.Rol).recibirNotificacion(mensaje);
                }               
            }
            db.SaveChanges(); // Guardar cambios en la BD

        }
    }
}