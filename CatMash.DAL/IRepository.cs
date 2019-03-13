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

        Task<IEnumerable<Cat>> GetCatsAsync<Parameters>(Parameters parameters)
            where Parameters : IBaseStoredProcedureParameters;
    }
}