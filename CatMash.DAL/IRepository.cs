using CatMash.Domain.Models;
using System.Threading.Tasks;

namespace CatMash.DAL
{
    public interface IRepository
    {
        Task<Cat> GetCatAsync(int id);
    }
}