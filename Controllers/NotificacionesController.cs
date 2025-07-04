using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Web.Mvc;
using vercom.Models;

public class NotificacionesController : Controller
{
    private VERCOMEntities db = new VERCOMEntities();

    public ActionResult Index()
    {
        var notifications = db.Notifications.Where(p=>p.IsRead == false).OrderByDescending(n => n.CreatedAt);
        return View(notifications.ToList());
    }

    public JsonResult EnviarNotificacion(string mensaje)
    {
        var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
        // Enviar la notificación a todos los clientes conectados
        hubContext.Clients.All.RecibirNotificacion(mensaje);
        return Json(new { mensaje = "Notificación enviada." }, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetUnreadNotifications(string username = "")
    {
        var rolname = db.UserRoles.Where(u => u.Users.UserName == username).Select(u => u.Roles.RoleName).FirstOrDefault();
        DateTime? fechaActual = System.DateTime.Now;
        var notificaciones = db.Notifications
            .Where(n => n.IsRead == false && n.Message.Contains("Nueva operación"))
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new { n.Id, n.Message, n.CreatedAt, n.Role })
            .ToList() // Ejecuta la consulta primero
            .Select(n => new {
                n.Id,
                n.Message,
                CreatedAt = n.CreatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ss"), // Formatear después de la ejecución
                n.Role
            }).ToList();
        if (rolname != "Administrador") notificaciones = notificaciones.Where(n => n.Role == rolname).ToList();    
        return Json(notificaciones, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetStockNotifications(string username = "")
    {
        var rolname = db.UserRoles.Where(u => u.Users.UserName == username).Select(u => u.Roles.RoleName).FirstOrDefault();
        DateTime? fechaActual = System.DateTime.Now;
        var notificaciones = db.Notifications
            .Where(n => n.IsRead == false && n.Message.Contains("bajo stock"))
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new { n.Id, n.Message, n.CreatedAt, n.Role })
            .ToList() // Ejecuta la consulta primero
            .Select(n => new {
                n.Id,
                n.Message,
                CreatedAt = n.CreatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ss"), // Formatear después de la ejecución
                n.Role
            }).ToList();
        if (rolname != "Admin") notificaciones = notificaciones.Where(n => n.Role == rolname).ToList();
        return Json(notificaciones, JsonRequestBehavior.AllowGet);
    }

    public JsonResult MarkNotificationAsRead(int id)
    {
        var notificacion = db.Notifications.Find(id);
        if (notificacion != null)
        {
            notificacion.IsRead = true;
            db.SaveChanges();
        }       
        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetMore(int page, int pageSize)
    {
        var notificaciones = db.Notifications
            .Where(n => n.IsRead == false) // Filtrar según tus necesidades
            .OrderByDescending(n => n.CreatedAt) // Ordenar por la fecha más reciente
            .Skip((page - 1) * pageSize) // Saltar los registros anteriores
            .Take(pageSize) // Tomar el número definido por pageSize
            .ToList() // Ejecutar la consulta y traer los datos a memoria
            .Select(n => new {
                n.Id,
                n.Message,
                CreatedAt = n.CreatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ss"), // Formatear después de la ejecución
                n.Role
            })
            .ToList(); // Transformar la lista final

        return Json(notificaciones, JsonRequestBehavior.AllowGet);
    }

    public JsonResult CleanOldNotifications()
    {
        var filterdate = DateTime.Now.AddDays(-7);
        var oldNotifications = db.Notifications.Where(n => n.CreatedAt < filterdate).ToList();
        if(oldNotifications != null)
        {
            db.Notifications.RemoveRange(oldNotifications);
            db.SaveChanges();
        }        
        return Json(oldNotifications, JsonRequestBehavior.AllowGet);
    }

    public JsonResult CleanAllNotifications()
    {       
        var allNotificacions = db.Notifications.ToList();
      
        if (allNotificacions != null)
        {
            db.Notifications.RemoveRange(allNotificacions);
            db.SaveChanges();
        }
        return Json(allNotificacions, JsonRequestBehavior.AllowGet);
    }
}
