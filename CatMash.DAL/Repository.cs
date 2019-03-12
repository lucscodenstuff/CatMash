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

        public async Task<Cat> GetCatAsync(int id)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Id", id);
            var catResult = new Cat();
            try
            {
                using (connection)
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryMultipleAsync("SelectOneCat", dynamicParameters, commandType: CommandType.StoredProcedure);

                    catResult = (await result.ReadFirstAsync<Cat>());
                    var enumFur = (await result.ReadAsync<FurTypesEnum>()).ToList();
                    catResult.FurTypes = enumFur;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return catResult;
        }

        public async Task<Response> GetOneAsync<Parameters, Response>(Parameters parameters) where Parameters : IBaseStoredProcedureParameters
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
