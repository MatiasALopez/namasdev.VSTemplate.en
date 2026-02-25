using System;

using namasdev.Data;
using namasdev.Data.Entity;

using MyApp.Data.Sql;
using MyApp.Entities;

namespace MyApp.Data
{
    public interface IErrorsRepository : IRepository<Error, Guid>
    {
    }

    public class ErrorsRepository : Repository<SqlContext, Error, Guid>, IErrorsRepository
    {
    }
}
