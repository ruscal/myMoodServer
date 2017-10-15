using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Web.Security;

namespace Discover.Security
{
    public class EntityFrameworkRoleProvider<TUserEntity, TRoleEntity> : RoleProvider
        where TUserEntity : class, IMembershipUser, new()
        where TRoleEntity : class, IRole, new()
    {
        protected Func<ObjectContext> getObjectContext = null;
        protected bool disposeObjectContextAfterUse = true;

        public void SetObjectContextSource(Func<ObjectContext> objectContextSourceFunc, bool disposeAfterUse)
        {
            this.getObjectContext = objectContextSourceFunc;
            this.disposeObjectContextAfterUse = disposeAfterUse;
        }

        protected string applicationName = "/";

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            var context = this.getObjectContext();
            try
            {
                return context.CreateObjectSet<TRoleEntity>().Any(r => r.Name == roleName);
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            var context = this.getObjectContext();
            try
            {
                return context.CreateObjectSet<TRoleEntity>().Any(r => r.Name == roleName && r.Users.Any(u => u.UserName == username));
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override string[] GetAllRoles()
        {
            var context = this.getObjectContext();
            try
            {
                return context.CreateObjectSet<TRoleEntity>().Select(r => r.Name).ToArray();
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var context = this.getObjectContext();
            try
            {
                return context.CreateObjectSet<TRoleEntity>().Where(r => r.Name == roleName).SelectMany(r => r.Users.Select(u => u.UserName)).ToArray();
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var context = this.getObjectContext();
            try
            {
                return context.CreateObjectSet<TRoleEntity>().Where(r => r.Users.Any(u => u.UserName == username)).Select(r => r.Name).ToArray();
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }

            if (string.IsNullOrEmpty(usernameToMatch))
            {
                return null;
            }

            var context = this.getObjectContext();
            try
            {
                return context.CreateObjectSet<TRoleEntity>().Where(r => r.Name == roleName).SelectMany(r => r.Users.Where(u => u.UserName.Contains(usernameToMatch)).Select(u => u.UserName)).ToArray();
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var context = this.getObjectContext();
                try
                {
                    var roles = context.CreateObjectSet<TRoleEntity>();

                    if (!roles.Any(r => r.Name == roleName))
                    {
                        roles.AddObject(new TRoleEntity() { Id = SequentialGuid.NewCombGuid(), Name = roleName });
                        context.SaveChanges();
                    }
                }
                finally
                {
                    if (disposeObjectContextAfterUse) context.Dispose();
                }
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            var context = this.getObjectContext();
            try
            {
                var roles = context.CreateObjectSet<TRoleEntity>();
                var role = roles.Where(r => r.Name == roleName).FirstOrDefault();

                if (role == null) return false;

                if (throwOnPopulatedRole && role.Users != null && role.Users.Any()) throw new InvalidOperationException("There are still users associated with the role \"" + roleName + "\"");

                roles.DeleteObject(role);
                context.SaveChanges();
                return true;
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>().Where(u => usernames.Contains(u.UserName)).ToArray();
                var roles = context.CreateObjectSet<TRoleEntity>().Where(r => roleNames.Contains(r.Name)).ToArray();

                foreach (var user in users)
                {
                    foreach (var role in roles)
                    {
                        role.Users.Add(user);
                    }
                }

                context.SaveChanges();
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var context = this.getObjectContext();
            try
            {
                var users = context.CreateObjectSet<TUserEntity>().Where(u => usernames.Contains(u.UserName)).ToArray();
                var roles = context.CreateObjectSet<TRoleEntity>().Where(r => roleNames.Contains(r.Name)).ToArray();

                foreach (var user in users)
                {
                    foreach (var role in roles)
                    {
                        role.Users.Remove(user);
                    }
                }

                context.SaveChanges();
            }
            finally
            {
                if (disposeObjectContextAfterUse) context.Dispose();
            }
        }
    }

    public interface IRole
    {
        Guid Id { get; set; }
        string Name { get; set; }

        ICollection<IMembershipUser> Users { get; }
    }
}
