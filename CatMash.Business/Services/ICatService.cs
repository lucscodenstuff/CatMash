using CatMash.Domain.Enums;
using CatMash.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatMash.Business.Services
{
    public interface ICatService
    {
        Task<Cat> RetrieveCatById(int id);
        Task<IEnumerable<Cat>> GetCats(FurTypesEnum? furType = null);
        Task<IEnumerable<Cat>> RetrieveTwoRandomCats(FurTypesEnum? furType = null);
        Task<Cat> PatchCats(Cat winner, Cat loser);
    }
}