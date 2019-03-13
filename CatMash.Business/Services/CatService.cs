using CatMash.DAL;
using CatMash.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatMash.Domain.Enums;
using CatMash.Domain.StoredProceduresParameters;

namespace CatMash.Business.Services
{
    public class CatService : ICatService
    {
        public readonly IRepository _repository;

        public CatService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Cat> RetrieveCatById(int id)
        {
            var parameters = new SelectOneCatParameters(id);
            return await _repository.GetCatAsync(parameters);
        }

        public async Task<IEnumerable<Cat>> GetCats(FurTypesEnum? furType = null)
        {
            var parameters = new SelectMultipleCatsParameters(furType: furType);
            return await _repository.GetCatsAsync(parameters);
        }

        public async Task<IEnumerable<Cat>> RetrieveTwoRandomCats(FurTypesEnum? furType = null)
        {
            var parameter = new SelectMultipleCatsParameters(furType: furType);
            var cats = (await _repository.GetCatsAsync(parameter)).OrderBy(x => x.ProbabilityWeight);

            var catOneId = ChoseCatContestant(cats);
            var catTwoId = ChoseCatContestant(cats);

            var parameters = new SelectTwoCatsParameters(catOneId, catTwoId, furType);
            return await _repository.GetCatsAsync(parameters);
        }

        private int ChoseCatContestant(IEnumerable<Cat> cats)
        {
            var random = new Random();
            double randomValue = random.NextDouble();
            double cumulativeProbability = 0.0;

            foreach (var cat in cats)
            {
                cumulativeProbability += cat.ProbabilityWeight;
                if (randomValue < cumulativeProbability)
                {
                    return cat.Id;
                }
            }
            return cats.Last().Id;
        }
    }
}
