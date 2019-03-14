using CatMash.Business.Services;
using CatMash.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatMash.Domain.Enums;

namespace CatMash.API.Controllers
{
    [Route("cats")]
    public class CatController : Controller
    {
        private readonly ICatService _catService;
        public CatController(ICatService catService)
        {
            _catService = catService;
        }
        [HttpGet, Route("{catId}", Name = "GetCat")]
        public async Task<Cat> GetCat(int catId)
        {
            return await _catService.RetrieveCatById(catId);
        }

        [HttpGet(Name = "GetCats")]
        public async Task<IEnumerable<Cat>> GetCats(FurTypesEnum? furTypes)
        {
            return new List<Cat>();
        }

        [HttpGet, Route("{furType}", Name = "GetCatsByFurType")]
        public async Task<IEnumerable<Cat>> GetCatsByFurType(FurTypesEnum furType)
        {
            return new List<Cat>();
        }

        [HttpGet, Route("random", Name = "GetTwoRandomCats")]
        public async Task<IEnumerable<Cat>> GetTwoRandomCats()
        {
            var cats = await _catService.RetrieveTwoRandomCats();
            return cats;
        }

        [HttpGet, Route("random/{furType}", Name = "GetTwoRandomCatsByFur")]
        public async Task<IEnumerable<Cat>> GetTwoRandomCatsByFur(FurTypesEnum furType)
        {
            return new List<Cat>();
        }

        [HttpPatch("{id}")]
        public async Task PatchCatsScores(Cat winner, Cat loser)
        {
            var winnerCat = await _catService.PatchCats(winner, loser);
        }
    }
}
