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
            return await _repository.GetAsync<Cat, SelectMultipleCatsParameters>(parameters);
        }

        public async Task<IEnumerable<Cat>> RetrieveTwoRandomCats(FurTypesEnum? furType = null)
        {
            var parameter = new SelectMultipleCatsParameters(furType: furType);
            var cats = (await _repository.GetAsync<Cat, SelectMultipleCatsParameters>(parameter)).OrderByDescending(x => x.ProbabilityWeight);

            var catOneId = ChoseCatContestant(cats);
            var catTwoId = GetAnotherCat(catOneId, cats);

            var parameters = new SelectTwoCatsParameters(catOneId, catTwoId, furType);
            return await _repository.GetAsync<Cat, SelectTwoCatsParameters>(parameters);
        }

        protected int ChoseCatContestant(IEnumerable<Cat> cats)
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

        internal int GetAnotherCat(int catOneId, IEnumerable<Cat> cats)
        {
            int catTwoId = ChoseCatContestant(cats);
            if (catOneId == catTwoId)
            {
                return GetAnotherCat(catOneId, cats);
            }
            return catTwoId;
        }

        public async Task<Cat> PatchWinnerCat(Cat winner)
        {
            var parameters = new CountViewsParameters();
            int totalViews = await _repository.GetOneAsync<int, CountViewsParameters>(parameters);
            if (totalViews == 0)
                totalViews = 1;

            var wins = Math.Round(winner.ViewsNumber * winner.Rating / 100) + 1;
            winner.ViewsNumber += 1;
            winner.ProbabilityWeight = 1 - (Convert.ToDouble(winner.ViewsNumber) / totalViews);
            winner.Rating = wins * 100 / winner.ViewsNumber;

            var updateParameter = new UpdateOneCatParameters(winner.Id, winner.ViewsNumber, winner.ProbabilityWeight, winner.Rating);
            winner = await _repository.GetCatAsync<UpdateOneCatParameters>(updateParameter);

            return winner;
        }

        public async Task<Cat> PatchLoserCat(Cat loser)
        {
            var parameters = new CountViewsParameters();
            int totalViews = await _repository.GetOneAsync<int, CountViewsParameters>(parameters);
            if (totalViews == 0)
                totalViews = 1;

            loser.ViewsNumber += 1;
            loser.ProbabilityWeight = 1 - (Convert.ToDouble(loser.ViewsNumber) / totalViews);

            var updateParameter = new UpdateOneCatParameters(loser.Id, loser.ViewsNumber, loser.ProbabilityWeight);
            loser = await _repository.GetCatAsync<UpdateOneCatParameters>(updateParameter);
            return loser;
        }
    }
}
