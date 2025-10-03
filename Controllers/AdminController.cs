using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using vercom.Class;
using vercom.Models;
using EntityState = System.Data.Entity.EntityState;
using Roles = vercom.Models.Roles;


namespace vercom.Controllers
{

    [Authorize]
    [RBAC]
    public class AdminController : Controller
    {
        private VERCOMEntities database = new VERCOMEntities();
        private UserRoleProvider rolmanager = new UserRoleProvider();

        #region Users
        // GET: Admin

        public ActionResult Index()
        {
            return View(database.Users.ToList());
        }


        public ActionResult UsersDetails(int id)
        {
            Users _users = database.Users.Find(id);
            SetViewBagData(id);
            return View(_users);
        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserCreate(Users Users)
        {
            if (Users.UserName == "" || Users.UserName == null)
            {
                ModelState.AddModelError(string.Empty, "UserName cannot be blank");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    List<string> results = database.Database.SqlQuery<String>(string.Format("SELECT UserName FROM Users WHERE UserName = '{0}'", Users.UserName)).ToList();
                    bool _UsersExistsInTable = (results.Count > 0);

                    Users _Users = null;
                    if (_UsersExistsInTable)
                    {
                        _Users = database.Users.Where(p => p.UserName == Users.UserName).FirstOrDefault();
                        if (_Users != null)
                        {
                            if (_Users.IsApproved == false)
                            {
                                ModelState.AddModelError(string.Empty, "Users already exists!");
                            }
                            else
                            {
                                database.Entry(_Users).Entity.IsApproved = false;
                                database.Entry(_Users).Entity.CreationDate = System.DateTime.Now;
                                database.Entry(_Users).State = EntityState.Modified;
                                database.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        _Users = new Users();
                        _Users.UserName = Users.UserName;
                        _Users.Email = Users.Email;

                        if (ModelState.IsValid)
                        {
                            _Users.IsApproved = false;
                            _Users.CreationDate = System.DateTime.Now;

                            database.Users.Add(_Users);
                            database.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Exception ex1 = ex;
            }

            return View();
        }
             
        public ActionResult UsersEdit(int id)
        {
            Users _users = database.Users.Find(id);
            SetViewBagData(id);
            return View(_users);
        }

        [HttpPost]
        public ActionResult UsersEdit(Users Users)
        {
            Users _Users = database.Users.Where(p => p.UserID == Users.UserID).FirstOrDefault();
            if (_Users == null)
            {
                return RedirectToAction("UsersDetails", new RouteValueDictionary(new { id = Users.UserID }));
            }
            try
            {
                database.Entry(_Users).CurrentValues.SetValues(_Users);
                database.SaveChanges();
            }
            catch (Exception)
            {

            }
            return RedirectToAction("UsersDetails", new RouteValueDictionary(new { id = Users.UserID }));
        }

        [HttpPost]
        public ActionResult UsersDetails(Users Users)
        {
            if (Users.UserName == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Users Name");
            }

            if (ModelState.IsValid)
            {
                database.Entry(Users).Entity.IsApproved = Users.IsApproved;
                database.Entry(Users).State = EntityState.Modified;
                database.SaveChanges();
            }
            return View(Users);
        }

        [HttpGet]
        public PartialViewResult filter4Users(string _surname)
        {
            return PartialView("_ListUsersTable", GetFilteredUsersList(_surname));
        }

        [HttpGet]
        public PartialViewResult filterReset()
        {
            return PartialView("_ListUsersTable", database.Users.ToList());
        }


        [HttpPost]
        public JsonResult DeleteMultipleUsers(int[] ids)
        {
            // 1. Eliminar roles asociados a los usuarios
            var rolesAsociados = database.UserRoles.Where(r => ids.Contains(r.UserID)).ToList();
            database.UserRoles.RemoveRange(rolesAsociados);

            // 2. Eliminar los usuarios
            var usuarios = database.Users.Where(u => ids.Contains(u.UserID)).ToList();
            database.Users.RemoveRange(usuarios);

            // 3. Guardar cambios
            database.SaveChanges();

            return Json(new { success = true });
        }

        private IEnumerable<Users> GetFilteredUsersList(string _surname)
        {
            IEnumerable<Users> _ret = null;
            try
            {
                if (string.IsNullOrEmpty(_surname))
                {
                    _ret = database.Users.Where(r => r.IsApproved == false).ToList();
                }
                else
                {
                    _ret = database.Users.Where(p => p.UserName == _surname).ToList();
                }
            }
            catch
            {
            }
            return _ret;
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUserRoleReturnPartialView(int id, int userId)
        {
            Roles role = database.Roles.Find(id);
            Users user = database.Users.Find(userId);
            var com = (from s in database.UserRoles where s.RoleID == id & s.UserID == userId select s).Any();
            if (!com)
            {
                UserRoles item = new UserRoles
                {
                    UserID = userId,
                    RoleID = role.RoleID,
                };
                role.UserRoles.Add(item);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.Users.Find(userId));
        }

        private void SetViewBagData(int _UsersId)
        {
            ViewBag.UsersId = _UsersId;
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            ViewBag.RoleId = new SelectList(database.Roles.OrderBy(p => p.RoleName), "RoleId", "RoleName");
        }

        public List<SelectListItem> List_boolNullYesNo()
        {
            var _retVal = new List<SelectListItem>();
            try
            {
                _retVal.Add(new SelectListItem { Text = "Not Set", Value = null });
                _retVal.Add(new SelectListItem { Text = "Yes", Value = bool.TrueString });
                _retVal.Add(new SelectListItem { Text = "No", Value = bool.FalseString });
            }
            catch { }
            return _retVal;
        }
        #endregion

        #region ROLES
        public ActionResult RoleIndex()
        {
            return View(database.Roles.OrderBy(r => r.RoleDescription).ToList());
        }

        public ViewResult RoleDetails(int id)
        {
            Users user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            Roles role = database.Roles.Where(r => r.RoleID == id)
                   .Include(a => a.RolePermissions)
                   .Include(a => a.UserRoles)
                   .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(database.Users.Where(r => r.Inactive == false || r.Inactive == null), "Id", "Username");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.Permissions.OrderBy(a => a.PermissionDescription), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(role);
        }

        public ActionResult RoleCreate()
        {
            Users user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View();
        }

        [HttpPost]
        public ActionResult RoleCreate(Roles _role)
        {
            if (_role.RoleDescription == null)
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            Users user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {


                database.Roles.Add(_role);
                database.SaveChanges();
                return RedirectToAction("RoleIndex");
            }
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleEdit(int id)
        {
            Users user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();

            Roles _role = database.Roles.Where(r => r.RoleID == id)
                    .Include(a => a.RolePermissions)
                    .Include(a => a.UserRoles)
                    .FirstOrDefault();

            // Users combo
            ViewBag.UsersId = new SelectList(database.Users.Where(r => r.IsApproved == false), "UserID", "UserName");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.Permissions.OrderBy(a => a.PermissionName), "PermissionID", "PermissionName");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(_role);
        }

        [HttpPost]
        public ActionResult RoleEdit(Roles _role)
        {
            if (string.IsNullOrEmpty(_role.RoleDescription))
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            //EntityState state = database.Entry(_role).State;
            Users user = database.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {

                database.Entry(_role).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("RoleDetails", new RouteValueDictionary(new { id = _role.RoleID }));
            }
            // USERS combo
            ViewBag.UserId = new SelectList(database.Users.Where(r => r.Inactive == false || r.Inactive == null), "UserID", "UserName");

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.Permissions.OrderBy(a => a.PermissionID), "PermissionID", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleDelete(int id)
        {
            Roles _role = database.Roles.Find(id);
            if (_role != null)
            {
                _role.UserRoles.Clear();
                _role.RolePermissions.Clear();

                database.Entry(_role).State = EntityState.Deleted;
                database.SaveChanges();
            }
            return RedirectToAction("RoleIndex");
        }

        [HttpPost]
        public JsonResult DeleteMultipleRoles(int?[] ids)
        {
            // 1. Eliminar usuarios asociados a rol
            var usuariosAsociados = database.UserRoles.Where(r => ids.Contains(r.RoleID)).ToList();
            database.UserRoles.RemoveRange(usuariosAsociados);

            // 1. Eliminar permisos asociados al rol
            var permisosAsociados = database.RolePermissions.Where(r => ids.Contains(r.RoleID)).ToList();
            database.RolePermissions.RemoveRange(permisosAsociados);

            // 2. Eliminar los roles
            var roles = database.Roles.Where(u => ids.Contains(u.RoleID)).ToList();
            database.Roles.RemoveRange(roles);

            // 3. Guardar cambios
            database.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeleteUserFromRole(int roleId, int userId)
        {
            // Buscar el rol en la base de datos
            var role = database.Roles.Find(roleId);
            if (role == null)
            {
                return Json(new { success = false, message = "El rol no existe en la base de datos." }, JsonRequestBehavior.AllowGet);
            }

            // Verificar si el rol tiene usuarios asociados
            if (role.UserRoles == null)
            {
                return Json(new { success = false, message = "El rol no tiene usuarios asociados." }, JsonRequestBehavior.AllowGet);
            }

            // Buscar la relación entre el usuario y el rol
            var userRoleToRemove = role.UserRoles.FirstOrDefault(ur => ur.UserID == userId);
            if (userRoleToRemove == null)
            {
                return Json(new { success = false, message = "El usuario no está asociado a este rol." }, JsonRequestBehavior.AllowGet);
            }

            // Remover la relación dentro de una transacción
            bool result = false;
            using (var transaction = database.Database.BeginTransaction())
            {
                try
                {
                    database.UserRoles.Remove(userRoleToRemove);
                    database.SaveChanges();
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = $"Error al eliminar la relación: {ex.Message}" }, JsonRequestBehavior.AllowGet);
                }
            }

            // Retornar el resultado al cliente
            return Json(new
            {
                success = result,
                message = result ? "La relación entre el usuario y el rol se eliminó exitosamente."
                                 : "No se pudo eliminar la relación."
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUser2RoleReturnPartialView(int id, int userId)
        {
            Roles role = database.Roles.Find(id);
            UserRoles user = database.UserRoles.Find(userId);

            if (!role.UserRoles.Contains(user))
            {
                role.UserRoles.Add(user);
                database.SaveChanges();
            }
            return PartialView("_ListUsersTable4Role", role);
        }

        #endregion

        #region PERMISSIONS

        public ViewResult PermissionIndex()
        {
            List<Permissions> _permissions = database.Permissions
                               .OrderBy(wn => wn.PermissionName)
                               .Include(a => a.RolePermissions)
                               .ToList();
            return View(_permissions);
        }

        public ViewResult PermissionDetails(int id)
        {
            Permissions _Permissions = database.Permissions.Find(id);
            return View(_Permissions);
        }

        public ActionResult PermissionCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PermissionCreate(Permissions _permission)
        {
            if (_permission.PermissionName == null)
            {
                ModelState.AddModelError("PermissionsDescription", "PermissionsDescription must be entered");
            }

            if (ModelState.IsValid)
            {
                database.Permissions.Add(_permission);
                database.SaveChanges();
                return RedirectToAction("PermissionIndex");
            }
            return View(_permission);
        }

        public ActionResult PermissionEdit(int id)
        {
            Permissions _permissions = database.Permissions.Find(id);
            ViewBag.RoleId = new SelectList(database.Roles.OrderBy(p => p.RoleName), "RoleID", "RoleName");
            return View(_permissions);
        }

        [HttpPost]
        public ActionResult PermissionEdit(Permissions _permission)
        {
            if (ModelState.IsValid)
            {
                database.Entry(_permission).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("PermissionDetails", new RouteValueDictionary(new { id = _permission.PermissionID }));
            }
            return View(_permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult PermissionDelete(int id)
        {
            bool result = false;
            Permissions permission = database.Permissions.Find(id);
            if (permission.RolePermissions.Count > 0)
            {
                permission.RolePermissions.Clear();
            }
            database.Entry(permission).State = EntityState.Deleted;
            database.SaveChanges();
            result = true;
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddPermission2RoleReturnPartialView(int id, int permissionId)
        {
            Roles role = database.Roles.Find(id);
            Permissions _permission = database.Permissions.Find(permissionId);
            var com = (from s in database.RolePermissions where s.RoleID == id & s.PermissionID == permissionId select s).Any();
            if (!com)
            {
                RolePermissions item = new RolePermissions
                {
                    PermissionID = permissionId,
                    RoleID = role.RoleID,
                };
                role.RolePermissions.Add(item);
                database.SaveChanges();
            }
            return PartialView("_ListPermissions", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddAllPermissions2RoleReturnPartialView(int id)
        {
            Roles _roles = database.Roles.Where(p => p.RoleID == id).FirstOrDefault();
            List<RolePermissions> _permissions = database.RolePermissions.ToList();
            foreach (RolePermissions _Permissions in _permissions)
            {
                if (!_roles.RolePermissions.Contains(_Permissions))
                {
                    _roles.RolePermissions.Add(_Permissions);

                }
            }
            database.SaveChanges();
            return PartialView("_ListPermissions", _roles);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult DeletePermissionFromRoleReturnPartialView(int id, int permissionId)
        {
            // Buscar el permiso en la base de datos
            var permiso = database.Permissions.Find(permissionId);
            if (permiso == null)
            {
                return Json(new { success = false, message = "El permiso no existe en la base de datos." }, JsonRequestBehavior.AllowGet);
            }

            // Verificar si el permiso tiene roles asociados
            if (permiso.RolePermissions == null)
            {
                return Json(new { success = false, message = "El permiso no tiene roles asociados." }, JsonRequestBehavior.AllowGet);
            }

            // Buscar la relación entre el permiso y el rol
            var rolePermission = permiso.RolePermissions.FirstOrDefault(ur => ur.RoleID == id);
            if (rolePermission == null)
            {
                return Json(new { success = false, message = "El permiso no está asociado a este rol." }, JsonRequestBehavior.AllowGet);
            }

            // Remover la relación dentro de una transacción
            bool result = false;
            using (var transaction = database.Database.BeginTransaction())
            {
                try
                {
                    database.RolePermissions.Remove(rolePermission);
                    database.SaveChanges();
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = $"Error al eliminar la relación: {ex.Message}" }, JsonRequestBehavior.AllowGet);
                }
            }

            // Retornar el resultado al cliente
            return Json(new
            {
                success = result,
                message = result ? "La relación entre el permiso y el rol se eliminó exitosamente."
                                 : "No se pudo eliminar la relación."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRoleFromPermissionReturnPartialView(int id, int permissionId)
        {
            Roles _roles = database.Roles.Find(id);
            RolePermissions _Permissions = database.RolePermissions.Find(permissionId);

            if (_roles.RolePermissions.Contains(_Permissions))
            {
                _roles.RolePermissions.Remove(_Permissions);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Permission", _Permissions);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRole2PermissionReturnPartialView(int permissionId, int roleId)
        {
            Roles role = database.Roles.Find(roleId);
            Permissions _permission = database.Permissions.Find(permissionId);
            var com = (from s in database.RolePermissions where s.RoleID == roleId & s.PermissionID == permissionId select s).Any();
            if (!com)
            {
                RolePermissions item = new RolePermissions
                {
                    PermissionID = permissionId,
                    RoleID = role.RoleID,
                };

                role.RolePermissions.Add(item);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Permission", _permission);
        }

        public ActionResult PermissionsImport()
        {
            var _controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic) // Ignorar ensamblados dinámicos
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        return new Type[0]; // Ignorar ensamblados que no se pueden cargar
                    }
                })
                .Where(t => t != null
                && t.IsPublic
                && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                && !t.IsAbstract
                && typeof(IController).IsAssignableFrom(t));

            var _controllerMethods = _controllerTypes.ToDictionary(controllerType => controllerType,
                    controllerType => controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType)));

            foreach (var _controller in _controllerMethods)
            {
                string _controllerName = _controller.Key.Name;
                foreach (var _controllerAction in _controller.Value)
                {
                    string _controllerActionName = _controllerAction.Name;
                    if (_controllerName.EndsWith("Controller"))
                    {
                        _controllerName = _controllerName.Substring(0, _controllerName.LastIndexOf("Controller"));
                    }

                    string _permissionDescription = string.Format("{0}-{1}", _controllerName, _controllerActionName);
                    Permissions _Permissions = database.Permissions.Where(p => p.PermissionName == _permissionDescription).FirstOrDefault();
                    if (_Permissions == null)
                    {
                        if (ModelState.IsValid)
                        {
                            Permissions _perm = new Permissions();
                            _perm.PermissionName = _permissionDescription;

                            database.Permissions.Add(_perm);
                            database.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("PermissionIndex");
        }
        #endregion
    }
}