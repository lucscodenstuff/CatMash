using CatMash.Business.Services;
using CatMash.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatMash.Domain.Enums;
using Microsoft.AspNetCore.Cors;

namespace CatMash.API.Controllers
{
    [EnableCors("AllowOrigins")]
    [Route("cats")]
    public class CatController : Controller
    {
        private readonly ICatService _catService;
        public CatController(ICatService catService)
        {
            _catService = catService;
        }

        [HttpGet, Route("{catId}", Name = "GetCat")]
        public async Task<IActionResult> GetCat(int catId)
        {
            var cat = await _catService.RetrieveCatById(catId);
            if (cat != null)
            {
                return Ok(cat);
            }

            return NotFound();
        }

        [HttpGet(Name = "GetCats")]
        public async Task<IActionResult> GetCats()
        {
            var cats = await _catService.GetCats();
            if (cats.Count() > 0 && cats != null)
            {
                return Ok(cats);
            }

            return NotFound();
        }

        [HttpGet, Route("{furType}", Name = "GetCatsByFurType")]
        public async Task<IActionResult> GetCatsByFurType([FromBody]FurTypesEnum furType)
        {
            var cats = await _catService.GetCats(furType);
            if (cats.Count() > 0 && cats != null)
            {
                return Ok(cats);
            }

            return NotFound();
        }

        [HttpGet, Route("random", Name = "GetTwoRandomCats")]
        public async Task<IActionResult> GetTwoRandomCats()
        {
            var cats = await _catService.RetrieveTwoRandomCats();
            if (cats.Count() > 0 && cats != null)
            {
                return Ok(cats);
            }

            return NotFound();
        }

        [HttpGet, Route("random/{furType}", Name = "GetTwoRandomCatsByFur")]
        public async Task<IActionResult> GetTwoRandomCatsByFur([FromBody]FurTypesEnum furType)
        {
            var cats = await _catService.RetrieveTwoRandomCats(furType);
            if (cats.Count() > 0 && cats != null)
            {
                return Ok(cats);
            }

            return NotFound();
        }

        [HttpPatch(Name = "PatchTwoCats")]
        public async Task<IActionResult> PatchCatsScores([FromBody] Payload cats)
        {
            var winner = cats.Winner;
            var loser = cats.Loser;
            var winnerCat = await _catService.PatchWinnerCat(winner);
            var loserCat = await _catService.PatchLoserCat(loser);
            if (winnerCat != null)
            {
                return CreatedAtRoute("GetCat", new { catId = winnerCat.Id}, winnerCat);
            }

            return BadRequest();
        }
    }
}
