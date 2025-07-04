using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using vercom.Models;

public class NotificacionesHub : Hub
{
    private VERCOMEntities db = new VERCOMEntities();

    // Este método envía notificaciones a los clientes
   
    public void recibirNotificacion(int userId, string message)
    {
        var Rol = db.UserRoles.Where(u => u.UserID == userId).Select(u => u.Roles.RoleName).FirstOrDefault();

        var notificacion = new Notifications
        {
            UserId = userId,
            Message = message,
            CreatedAt = DateTime.Now,
            Role = Rol,
            IsRead = false
        };

        var notificacionDto = new
        {
            UserId = notificacion.UserId,
            Message = notificacion.Message,
            CreatedAt = notificacion.CreatedAt,
            Role = notificacion.Role
        };

        db.Notifications.Add(notificacion);
        db.SaveChanges();       
        Clients.All.RecibirNotificacion(notificacionDto);
    }

    public void SendPersistentNotification(string username, string message)
    {
        var userId = db.Users.Where(u => u.UserName == username).Select(u => u.UserID).FirstOrDefault();
        var Rol = db.UserRoles.Where(u => u.Users.UserName == username).Select(u => u.Roles.RoleName).FirstOrDefault();
        var notificacion = new Notifications
        {
            UserId = userId,
            Message = message,
            CreatedAt = DateTime.Now,
            Role = Rol,
            IsRead = false
        };
        db.Notifications.Add(notificacion);
        db.SaveChanges();
        Clients.Group(username).receiveNotification(message);
    }

    public List<Notifications> GetUnreadNotifications(int userId)
    {
        return db.Notifications.Where(n =>  n.UserId == userId && n.IsRead == false).ToList();
    }

    // Registrar la conexión del usuario
    public override Task OnConnected()
    {
        string userName = Context.User.Identity.Name; // Obtener usuario autenticado
        var userRol = db.UserRoles.Where(x => x.Users.UserName == userName).Select(x => x.Roles.RoleName).FirstOrDefault();// Obtener rol del usuario
        if (!string.IsNullOrEmpty(userRol))
        {
            Groups.Add(Context.ConnectionId, userRol); // Agregar el usuario al grupo de su rol
        }
        return base.OnConnected();
    }

    public async Task SendStockAlert(string message)
    {
        await Clients.Group("Admin").SendAsync("ReceiveStockAlert", message);
    }
    public void SendNotificationToGroup(string userName, string message)
    {
        var Rol = db.UserRoles.Where(u => u.Users.UserName == userName).Select(u => u.Roles.RoleName).FirstOrDefault();
        Clients.Group(Rol).receiveNotification(message);
    }

    public void SendNotificationToAdmins(string message)
    {
        Clients.Group("Administradores").receiveNotification(message);
    }
     
}
