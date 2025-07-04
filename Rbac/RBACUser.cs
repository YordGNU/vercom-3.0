using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using vercom.Models;

public class RBACUser
{
    public int User_Id { get; set; }
    public Nullable<bool> IsSysAdmin { get; set; }
    public string Username { get; set; }
    private List<UserRole> Roles = new List<UserRole>();

    public RBACUser(string _username)
    {
        this.Username = _username;
        this.IsSysAdmin = false;
        GetDatabaseUserRolesPermissions();
    }

    private void GetDatabaseUserRolesPermissions()
    {
        using (VERCOMEntities _data = new VERCOMEntities())
        {
            var _user = _data.Users.Where(u => u.UserName == this.Username).FirstOrDefault();
            if (_user != null)
            {               
                this.User_Id = _user.UserID;
                foreach (var _role in _user.UserRoles)
                {
                    UserRole _userRole = new UserRole { Role_Id = _role.Roles.RoleID, RoleName = _role.Roles.RoleName };
                    foreach (var _permission in _role.Roles.RolePermissions)
                    {
                        _userRole.Permissions.Add(new RolePermission { Permission_Id = _permission.Permissions.PermissionID, PermissionDescription = _permission.Permissions.PermissionDescription });
                    }
                    this.Roles.Add(_userRole);
                    this.IsSysAdmin = _role.Roles.IsSysAdmin;
                }                
            }
        }
    }

    public bool HasPermission(string requiredPermission)
    {
        bool bFound = false;
        using (VERCOMEntities _data = new VERCOMEntities())
        {
            foreach (UserRole role in this.Roles)
            {
                bFound = (from r in _data.RolePermissions where (r.Roles.RoleID == role.Role_Id && r.Permissions.PermissionName == requiredPermission) select r).Any();
                if (bFound)
                    break;
            }
        }

        return bFound;
    }

    public bool HasRole(string role)
    {
        return (Roles.Where(p => p.RoleName == role).ToList().Count > 0);
    }

    public bool HasRoles(string roles)
    {
        bool bFound = false;
        string[] _roles = roles.ToLower().Split(';');
        foreach (UserRole role in this.Roles)
        {
            try
            {
                bFound = _roles.Contains(role.RoleName.ToLower());
                if (bFound)
                    return bFound;
            }
            catch (Exception)
            {
            }
        }
        return bFound;
    }
}

public class UserRole
{
    public int Role_Id { get; set; }
    public string RoleName { get; set; }
    public List<RolePermission> Permissions = new List<RolePermission>();
}

public class RolePermission
{
    public int Permission_Id { get; set; }
    public string PermissionDescription { get; set; }
}