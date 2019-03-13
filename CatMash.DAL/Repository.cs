using CatMash.Domain;
using CatMash.Domain.Enums;
using CatMash.Domain.Models;
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

        public async Task<Cat> GetCatAsync<Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            string storedProcedure = null;

            var dynamicParameters = CreateParameters(parameters, out storedProcedure);

            var cat = new Cat();
            try
            {
                if (!String.IsNullOrEmpty(storedProcedure))
                {
                    using (connection)
                    {
                        await connection.OpenAsync();
                        var result = await connection.QueryMultipleAsync(storedProcedure, dynamicParameters,
                            commandType: CommandType.StoredProcedure);

                        cat = (await result.ReadFirstAsync<Cat>());
                        var enumFur = (await result.ReadAsync<FurTypesEnum>()).ToList();
                        cat.FurTypes = enumFur;
                    }
                    return cat;
                }
                throw new ArgumentException("The stored procedure name is missing");
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error calling {storedProcedure} : \r\n" + e.ToString());
                throw;
            }
        }     

        public async Task<IEnumerable<Cat>> GetCatsAsync<Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            string storedProcedure = null;

            var dynamicParameters = CreateParameters(parameters, out storedProcedure);

            IEnumerable<Cat> cats;
            try
            {
                if (!String.IsNullOrEmpty(storedProcedure))
                {
                    using (connection)
                    {
                        await connection.OpenAsync();
                        cats = await connection.QueryAsync<Cat>(storedProcedure, dynamicParameters,
                            commandType: CommandType.StoredProcedure);
                    }
                    return cats;
                }
                throw new ArgumentException("The stored procedure name is missing");
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error calling {storedProcedure} : \r\n" + e.ToString());
                throw;
            }
        }

        private DynamicParameters CreateParameters<T>(T parameters, out string storedProcedure) where T : IBaseStoredProcedureParameters
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
