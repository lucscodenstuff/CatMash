using CatMash.Business.Services;
using CatMash.DAL;
using CatMash.Domain.Models;
using CatMash.Domain.StoredProceduresParameters;
using NFluent;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CatMash.Test
{
    public class CatServiceTests
    {
        private readonly ICatService _catService;
        private readonly IRepository _repository;

        public CatServiceTests()
        {
            _repository = Substitute.For<IRepository>();
            _catService = new CatService(_repository);
        }

        [Fact]
        public async Task Givent_AWinnerCat_When_PatchingIt_Then_CallTheDBWithTheRightsArumentsAndReturnTheUpdatedWinner()
        {
            int totalViews = 20;
            var winner = new Cat
            {
                Id = 1,
                ViewsNumber = 7,
                ProbabilityWeight = 0.65,
                Rating = 42.85
            };

            var winnerAfterMatch = new Cat
            {
                Id = 1,
                ViewsNumber = 8,
                ProbabilityWeight = 0.6,
                Rating = 50
            };
            var updateParameter = new UpdateOneCatParameters(winnerAfterMatch.Id, winnerAfterMatch.ViewsNumber, winnerAfterMatch.ProbabilityWeight, winnerAfterMatch.Rating);

            _repository.GetOneAsync<int, CountViewsParameters>(Arg.Any<CountViewsParameters>()).Returns(totalViews);
            _repository.GetCatAsync<UpdateOneCatParameters>(Arg.Any<UpdateOneCatParameters>()).Returns(winnerAfterMatch);

            var result = await _catService.PatchWinnerCat(winner);

            Check.That(result).HasFieldsWithSameValues(winnerAfterMatch);

            await _repository.Received().GetCatAsync<UpdateOneCatParameters>(Arg.Is<UpdateOneCatParameters>(x => (x.Id == 1 && x.Views == 8 && x.Weight == 0.6 && x.Rating == 50)));

        }
        [Fact]
        public async Task Givent_ALoserCat_When_PatchingIt_Then_CallTheDBWithTheRightsArumentsAndReturnTheUpdatedWinner()
        {
            int totalViews = 20;
            var loser = new Cat
            {
                Id = 1,
                ViewsNumber = 7,
                ProbabilityWeight = 0.65,
                Rating = 42.85
            };

            var loserAfterMatch = new Cat
            {
                Id = 1,
                ViewsNumber = 8,
                ProbabilityWeight = 0.6,
            };
            var updateParameter = new UpdateOneCatParameters(loserAfterMatch.Id, loserAfterMatch.ViewsNumber, loserAfterMatch.ProbabilityWeight);

            _repository.GetOneAsync<int, CountViewsParameters>(Arg.Any<CountViewsParameters>()).Returns(totalViews);
            _repository.GetCatAsync<UpdateOneCatParameters>(Arg.Any<UpdateOneCatParameters>()).Returns(loserAfterMatch);

            var result = await _catService.PatchLoserCat(loser);

            Check.That(result).HasFieldsWithSameValues(loserAfterMatch);

            await _repository.Received().GetCatAsync<UpdateOneCatParameters>(Arg.Is<UpdateOneCatParameters>(x => (x.Id == 1 && x.Views == 8 && x.Weight == 0.6 && x.Rating == null)));

        }
    }
}
