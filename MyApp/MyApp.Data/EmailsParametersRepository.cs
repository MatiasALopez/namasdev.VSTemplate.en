using namasdev.Data;
using namasdev.Data.Entity;

using MyApp.Data.Sql;
using MyApp.Entities;

namespace MyApp.Data
{
    public interface IEmailsParametersRepository : IReadOnlyRepository<EmailParameters, short>
    {
    }

    public class EmailsParametersRepository : ReadOnlyRepository<SqlContext, EmailParameters, short>, IEmailsParametersRepository
    {
    }
}
