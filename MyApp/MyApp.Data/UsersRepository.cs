using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using namasdev.Core.Linq;
using namasdev.Core.Exceptions;
using namasdev.Data;
using namasdev.Data.Entity;

using MyApp.Data.Sql;
using MyApp.Entities;
using MyApp.Entities.Values;

namespace MyApp.Data
{
    public interface IUsersRepository : IRepository<User, string>
    {
        List<string> GetEmailsByRol(string rolNombre);
        string GetFullNameById(string usuarioId);
        User Get(string userId, bool loadRoles = false, bool includeDeleted = false);
        bool ExistsDeletedByEmail(string email, out string userId);
        string[] GetRoles();
        List<User> GetList(bool? deleted = null, string search = null, string role = null, bool loadRoles = false, OrderAndPagingParameters op = null);
        string GetUserRole(string userId);
    }

    public class UsersRepository : Repository<SqlContext, User, string>, IUsersRepository
    {
        public override void Add(User entity)
        {
            try
            {
                base.Add(entity);
            }
            catch (Exception)
            {
                ValidateUniqueEmail(entity);
                
                throw;
            }
        }

        public override void Update(User entity)
        {
            try
            {
                base.Update(entity);
            }
            catch (Exception)
            {
                ValidateUniqueEmail(entity);

                throw;
            }
        }

        public List<string> GetEmailsByRol(string roleName)
        {
            using (var ctx = BuildContext())
            {
                return EntitySet(ctx)
                    .Where(u =>
                        u.Roles.Any(r => r.Name == roleName)
                        && !u.Deleted)
                    .Select(u => u.Email)
                    .Distinct()
                    .ToList();
            }
        }

        public string GetFullNameById(string userId)
        {
            using (var ctx = BuildContext())
            {
                return EntitySet(ctx)
                    .Where(u => u.Id == userId && !u.Deleted)
                    .Select(u => u.FullName)
                    .FirstOrDefault();
            }
        }

        public User Get(string userId,
            bool loadRoles = false,
            bool includeDeleted = false)
        {
            using (var ctx = BuildContext())
            {
                return EntitySet(ctx)
                    .IncludeIf(u => u.Roles, loadRoles)
                    .Where(e => e.Id == userId)
                    .WhereIf(u => !u.Deleted, !includeDeleted)
                    .FirstOrDefault();
            }
        }

        public bool ExistsDeletedByEmail(string email,
            out string userId)
        {
            using (var ctx = BuildContext())
            {
                userId = EntitySet(ctx)
                    .Where(u => u.Email == email && u.Deleted)
                    .Select(u => u.Id)
                    .FirstOrDefault();

                return !String.IsNullOrWhiteSpace(userId);
            }
        }

        public string[] GetRoles()
        {
            return new[] { AspNetRoles.ADMINISTRATOR };
        }

        public List<User> GetList(
            bool? deleted = false, string search = null, string role = null,
            bool loadRoles = false,
            OrderAndPagingParameters op = null)
        {
            using (var ctx = BuildContext())
            {
                return EntitySet(ctx)
                    .IncludeIf(u => u.Roles, loadRoles)
                    .WhereIf(u =>
                        u.FullNameInverted.Contains(search)
                        || u.Email.Contains(search),
                        !String.IsNullOrWhiteSpace(search))
                    .WhereIf(u => u.Roles.Any(r => r.Name == role), !String.IsNullOrWhiteSpace(role))
                    .WhereIf(u => u.Deleted == deleted, deleted.HasValue)
                    .OrderAndPage(op, defaultOrder: nameof(User.FullNameInverted))
                    .ToList();
            }
        }

        public string GetUserRole(string userId)
        {
            using (var ctx = BuildContext())
            {
                return EntitySet(ctx)
                    .Where(u => u.Id == userId)
                    .Select(u => u.Roles.Select(ur => ur.Name).FirstOrDefault())
                    .FirstOrDefault();
            }
        }

        private void ValidateUniqueEmail(User user)
        {
            if (ExistsByEmail(user.Email, excludeUserId: user.Id))
            {
                throw new ExceptionFriendlyMessage("The email is already taken by another user.");
            }
        }

        private bool ExistsByEmail(string email,
            string excludeUserId = null)
        {
            using (var ctx = this.BuildContext())
            {
                return EntitySet(ctx)
                    .Where(u => u.Email == email)
                    .WhereIf(u => u.Id != excludeUserId, !string.IsNullOrWhiteSpace(excludeUserId))
                    .Any();
            }
        }
    }
}
