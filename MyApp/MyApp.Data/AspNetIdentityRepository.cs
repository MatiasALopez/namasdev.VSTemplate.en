using System;
using System.Data.SqlClient;

using MyApp.Data.Sql;

namespace MyApp.Data
{
    public interface IAspNetIdentityRepository
    {
        void AddOrUpdateUsersIdentity(string userId, string roleName = null);
        void UnlockUser(string userId);
    }

    public class AspNetIdentityRepository : IAspNetIdentityRepository
    {
        public void AddOrUpdateUsersIdentity(string userId, 
            string roleName = null)
        {
            using (var ctx = new SqlContext())
            {
                ctx.usp_AddOrUpdateUsersIdentity(userId, 
                    roleName: roleName);
            }
        }

        public void UnlockUser(string userId)
        {
            using (var ctx = new SqlContext())
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Date", new DateTime(1900,1,1)),
                };

                ctx.ExecuteCommand(
                    "UPDATE dbo.AspNetUsers SET LockoutEndDateUtc = @Date WHERE Id = @UserId",
                    parameters);
            }
        }
    }
}
