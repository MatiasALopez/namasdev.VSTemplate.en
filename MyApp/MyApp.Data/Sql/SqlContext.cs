using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

using CodeFirstStoreFunctions;
using namasdev.Data.Entity;

using MyApp.Entities;

namespace MyApp.Data.Sql
{
    public class SqlContext : DbContextBase
    {
        private const string FUNCTIONS_NAMESPACE = "SqlContext";

        public SqlContext()
            : base("name=MyApp")
        {
            Configuration.LazyLoadingEnabled = false;
            Database.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["BDCommandTimeoutInSec"]);
        }

        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<EmailParameters> EmailParameters { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            IgnoreTypes(modelBuilder);

            // NOTE (ML): uncomment next line when necessary
            //modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
            RegisterComplexTypes(modelBuilder);

            modelBuilder.Conventions.Add(new FunctionsConvention<SqlContext>("dbo"));

            base.OnModelCreating(modelBuilder);
        }

        private void IgnoreTypes(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<TypeA>();
        }

        private void RegisterComplexTypes(DbModelBuilder modelBuilder)
        {
            //modelBuilder.ComplexType<TypeA>();
        }

        public string GetUserRole(string userId)
        {
            return uf_GetIdentityUserRoles(userId)
                .Select(r => r.Name)
                .FirstOrDefault();
        }

        #region Functions

        [DbFunction(FUNCTIONS_NAMESPACE, nameof(uf_GetIdentityUserRoles))]
        public IQueryable<AspNetRole> uf_GetIdentityUserRoles(string userId)
        {
            var pUserId = new ObjectParameter("UserId", userId);

            return ((IObjectContextAdapter)this).ObjectContext
                .CreateQuery<AspNetRole>(
                    $"{FUNCTIONS_NAMESPACE}.{nameof(uf_GetIdentityUserRoles)}(@UserId)",
                    pUserId);
        }

        //public int ufn_FunctionScalar()
        //{
        //    return Database
        //        .SqlQuery<int>("SELECT [dbo].[ufn_FunctionScalar]()")
        //        .Single();
        //}
        #endregion

        #region Stored Procedures

        public void usp_AddOrUpdateUsersIdentity(string userId, 
            string roleName = null)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@UserId", (object)userId ?? DBNull.Value),
                new SqlParameter("@RoleName", (object)roleName ?? DBNull.Value),
            };

            this.Database.ExecuteSqlCommand(
                TransactionalBehavior.DoNotEnsureTransaction, 
                $"exec dbo.usp_AddOrUpdateUsersIdentity @UserId,@RoleName", 
                parametros);
        }

        //public IEnumerable<TypeA> usp_StoredProcedureA(string param1, Guid? param2 = null)
        //{
        //    var p1 = new SqlParameter("Param1", param1);

        //    var p2 =
        //        param2.HasValue
        //        ? new SqlParameter("Param2", param2)
        //        : new SqlParameter("Param2", typeof(Guid));

        //    return Database.SqlQuery<TypeA>(
        //        "EXEC dbo.usp_StoredProcedureA @Param1,@Param2",
        //        new[] { p1, p2 })
        //        .ToList();
        //}

        //public void usp_StoredProcedureB(
        //    string param1, 
        //    out int paramOut,
        //    Guid? param2 = null)
        //{
        //    var pOut = new SqlParameter("@ParamOut", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //    var p1 = new SqlParameter("Param1", param1);

        //    var p2 =
        //        param2.HasValue
        //        ? new SqlParameter("Param2", param2)
        //        : new SqlParameter("Param2", typeof(Guid));

        //    this.Database.ExecuteSqlCommand(
        //        TransactionalBehavior.DoNotEnsureTransaction, 
        //        $"exec dbo.usp_StoredProcedureB @Param1, @Param2, @ParamOut OUT", 
        //        new[] { p1, p2, pOut });

        //    paramOut = (int)pOut.Value;
        //}

        #endregion
    }
}
