using CatMash.Domain.Models;
using CatMash.Domain.StoredProceduresParameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatMash.DAL
{
    public interface IRepository
    {
        Task<Cat> GetCatAsync<Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters;

        Task<IEnumerable<Response>> GetAsync<Response, Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters;

        Task<Response> GetOneAsync<Response, Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters;
    }
}