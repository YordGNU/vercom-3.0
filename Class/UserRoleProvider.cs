using System;
using System.Data;
using System.Linq;
using System.Web.Security;
using vercom.Models;

namespace vercom.Class
{
    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var context = new VERCOMEntities())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == username);
                return user != null && user.UserRoles.Any(r => r.Roles.RoleName == roleName);
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var context = new VERCOMEntities())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == username);
                return user == null ? new string[] { } : user.UserRoles.Select(r => r.Roles.RoleName).ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            using (var context = new VERCOMEntities())
            {
                if (!context.Roles.Any(r => r.RoleName == roleName))
                {
                    var role = new Models.Roles { RoleName = roleName };
                    context.Roles.Add(role);
                    context.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException("Role already exists.");
                }
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var context = new VERCOMEntities())
            {
                foreach (var username in usernames)
                {
                    var user = context.Users.FirstOrDefault(u => u.UserName == username);
                    if (user != null)
                    {
                        foreach (var roleName in roleNames)
                        {
                            if (!user.UserRoles.Any(r => r.Roles.RoleName == roleName))
                            {
                                var role = context.UserRoles.FirstOrDefault(r => r.Roles.RoleName == roleName);
                                if (role != null)
                                {
                                    user.UserRoles.Add(role);
                                }
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var context = new VERCOMEntities())
            {
                foreach (var username in usernames)
                {
                    var user = context.Users.FirstOrDefault(u => u.UserName == username);
                    if (user != null)
                    {
                        foreach (var roleName in roleNames)
                        {
                            var role = user.UserRoles.FirstOrDefault(r => r.Roles.RoleName == roleName);
                            if (role != null)
                            {
                                user.UserRoles.Clear();
                                user.UserRoles.Remove(role);
                            }
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (var context = new VERCOMEntities())
            {
                return context.Roles.Any(r => r.RoleName == roleName);
            }
        }

        public override string[] GetAllRoles()
        {
            using (var context = new VERCOMEntities())
            {
                return context.Roles.Select(r => r.RoleName).ToArray();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
        // Implement other required methods with similar structure...
    }

}