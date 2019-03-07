using CatMash.Domain;
using CatMash.Domain.Enums;
using CatMash.Domain.StoredProceduresParameters;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CatMash.DAL
{
    public class Repository : IRepository
    {
        private readonly IConfiguration _configuration;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Response> GetOneAsync<Parameters, Response>(Parameters parameters) where Parameters : BaseStoredProcedureParameters
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var dynamicParameters = new DynamicParameters();
            string storedProcedure = null;
            Response result;

            dynamicParameters = CreateParameters(parameters, out storedProcedure);

            try
            {
                if (!String.IsNullOrEmpty(storedProcedure))
                {
                    using (connection)
                    {
                        await connection.OpenAsync();
                        result = (await connection.QueryAsync<Response>(storedProcedure, dynamicParameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    }
                    return result;
                }
                throw new ArgumentException("The stored procedure name is missing");
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error calling {storedProcedure} : \r\n" + e.ToString());
                throw;
            }

        }

        private DynamicParameters CreateParameters<T>(T parameters, out string storedProcedure) where T : BaseStoredProcedureParameters
        {
            var dynamicParameters = new DynamicParameters();
            storedProcedure = null;
            var parametersType = parameters.GetType();

            var properties = new List<PropertyInfo>(parametersType.GetProperties());

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(StoredProceduresEnum))
                {
                    storedProcedure = property.GetValue(parameters).ToString();
                }
                else if (property.GetValue(parameters) != null)
                {
                    dynamicParameters.Add($"@{property.Name}", property.GetValue(parameters));
                }
            }

            return dynamicParameters;
        }
    }
}
