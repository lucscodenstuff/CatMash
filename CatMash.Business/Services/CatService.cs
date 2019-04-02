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
            return (await _repository.GetAsync<Cat, SelectMultipleCatsParameters>(parameters)).OrderByDescending(x => x.ProbabilityWeight);
        }

        public async Task<IEnumerable<Cat>> RetrieveTwoRandomCats(FurTypesEnum? furType = null)
        {
            var parameter = new SelectMultipleCatsParameters(furType: furType);
            var cats = (await _repository.GetAsync<Cat, SelectMultipleCatsParameters>(parameter)).OrderBy(x => x.ProbabilityWeight);

            var catOneId = await ChoseCatContestant(cats);
            var catTwoId = await GetAnotherCat(catOneId, cats);

            var parameters = new SelectTwoCatsParameters(catOneId, catTwoId, furType);
            return await _repository.GetAsync<Cat, SelectTwoCatsParameters>(parameters);
        }

        protected async Task<int> ChoseCatContestant(IEnumerable<Cat> cats)
        {
            var parameters = new GetTotalWeightParameters();
            double totalWeight = await _repository.GetOneAsync<double, GetTotalWeightParameters>(parameters);
            var random = new Random();
            double randomValue = random.NextDouble() * totalWeight;
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

        internal async Task<int> GetAnotherCat(int catOneId, IEnumerable<Cat> cats)
        {
            int catTwoId = await ChoseCatContestant(cats);
            if (catOneId == catTwoId)
            {
                return await GetAnotherCat(catOneId, cats);
            }
            return catTwoId;
        }

        public async Task<Cat> PatchWinnerCat(Cat winner)
        {
            var parameters = new CountViewsParameters();
            int totalViews = await _repository.GetOneAsync<int, CountViewsParameters>(parameters);
            if (totalViews == 0)
                totalViews = 1;
            winner.ViewsNumber += 1;
            winner.Wins += 1;

            winner.ProbabilityWeight = await CalculateWeight(winner.ViewsNumber, totalViews);

            winner.Rating = winner.Wins * 100 / winner.ViewsNumber;

            var updateParameter = new UpdateOneCatParameters(winner.Id, winner.ViewsNumber, winner.ProbabilityWeight, winner.Rating, winner.Wins);
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

            loser.ProbabilityWeight = await CalculateWeight(loser.ViewsNumber, totalViews);

            loser.Rating = loser.Wins * 100 / loser.ViewsNumber;

            var updateParameter = new UpdateOneCatParameters(loser.Id, loser.ViewsNumber, loser.ProbabilityWeight, loser.Rating);
            loser = await _repository.GetCatAsync<UpdateOneCatParameters>(updateParameter);
            return loser;
        }

        private async Task<double> CalculateWeight(int views, int totalViews)
        {
            double theoricProbability = Convert.ToDouble(totalViews) / 100;
            double theoricWeight = Convert.ToDouble(views) / theoricProbability;

            if (theoricWeight >= 1)
            {
                return 0.01;
            }
            else
            {
                return 1 - theoricWeight;
            }
        }
    }
}
