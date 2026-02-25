using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAzure.Storage;
using namasdev.Core.Validation;

using MyApp.Data.Sql;
using MyApp.Entities;
using MyApp.Entities.Values;

namespace MyApp.Data
{
    public interface IParametersRepository
    {
        string Get(string name);
        Dictionary<string, string> Get(params string[] names);
        void AddOrUpdate(string name, string value);
        void AddOrUpdate(Dictionary<string, string> parameters);
        CloudStorageAccount GetCloudStorageAccount();
    }

    public class ParametersRepository : IParametersRepository
    {
        public string Get(string name)
        {
            using (var ctx = new SqlContext())
            {
                return ctx.Parameters
                    .Where(e => e.Name == name)
                    .Select(e => e.Value)
                    .FirstOrDefault();
            }
        }

        public Dictionary<string, string> Get(params string[] names)
        {
            if (names == null || !names.Any())
            {
                return new Dictionary<string, string>();
            }

            using (var ctx = new SqlContext())
            {
                return ctx.Parameters
                    .Where(e => names.Contains(e.Name))
                    .ToDictionary(e => e.Name, e => e.Value);
            }
        }

        public void AddOrUpdate(string name, string value)
        {
            AddOrUpdate(new Dictionary<string, string> { { name, value } });
        }

        public void AddOrUpdate(Dictionary<string, string> parameters)
        {
            Validator.ValidateRequiredListArgumentAndThrow(parameters, nameof(parameters));

            using (var ctx = new SqlContext())
            {
                Parameter p = null;
                foreach (var parameter in parameters)
                {
                    p = ctx.Parameters.Find(parameter.Key);

                    if (p == null)
                    {
                        p = new Parameter { Name = parameter.Key };
                        ctx.Parameters.Add(p);
                    }

                    p.Value = parameter.Value;
                }

                ctx.SaveChanges();
            }
        }

        public CloudStorageAccount GetCloudStorageAccount()
        {
            return CloudStorageAccount.Parse(Get(Parameters.CLOUD_STORAGE_ACCOUNT_CONNECTION_STRING));
        }
    }
}