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
    public abstract class Repository : IRepository
    {
        private readonly IConfiguration _configuration;

        protected Repository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Cat> GetCatAsync<Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var dynamicParameters = CreateParameters(parameters, out var storedProcedure);

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
                Trace.TraceError($"Error calling {storedProcedure} : \r\n" + e);
                throw;
            }
        }     

        public async Task<IEnumerable<Response>> GetAsync<Response, Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var dynamicParameters = CreateParameters(parameters, out var storedProcedure);

            try
            {
                if (!String.IsNullOrEmpty(storedProcedure))
                {
                    IEnumerable<Response> results;
                    using (connection)
                    {
                        await connection.OpenAsync();
                        results = await connection.QueryAsync<Response>(storedProcedure, dynamicParameters,
                            commandType: CommandType.StoredProcedure);
                    }
                    return results;
                }
                throw new ArgumentException("The stored procedure name is missing");
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error calling {storedProcedure} : \r\n" + e);
                throw;
            }
        }

        public async Task<Response> GetOneAsync<Response, Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var dynamicParameters = CreateParameters(parameters, out var storedProcedure);

            try
            {
                if (!String.IsNullOrEmpty(storedProcedure))
                {
                    Response result;
                    using (connection)
                    {
                        await connection.OpenAsync();
                        result = (await connection.QueryAsync<Response>(storedProcedure, dynamicParameters,
                            commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    }
                    return result;
                }
                throw new ArgumentException("The stored procedure name is missing");
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error calling {storedProcedure} : \r\n" + e);
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
