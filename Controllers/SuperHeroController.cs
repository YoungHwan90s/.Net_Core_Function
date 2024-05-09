using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NetCoreWebAPI.Interfaces.Repositories;
using NetCoreWebAPI.Models;

namespace NetCoreWebAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly ISuperHeroRepository _superHeroRepository;

        public SuperHeroController(ISuperHeroRepository superHeroRepository)
        {
            _superHeroRepository = superHeroRepository;
        }

        [HttpPost]
        [Route("addSuperHeros")]
        public async Task<ActionResult> AddSuperHeros()
        {
            var superHeros = new List<SuperHeros>
            {
                new SuperHeros
                {
                    SuperHero = "Spiderman",
                    Name = "Peter",
                    PowerIndex = 10
                },
                new SuperHeros
                {
                    SuperHero = "Batman",
                    Name = "Bruce",
                    PowerIndex = 9
                },
                new SuperHeros
                {
                    SuperHero = "Superman",
                    Name = "Clark",
                    PowerIndex = 11
                }
            };

            await _superHeroRepository.AddSuperHero(superHeros);

            return Ok(superHeros);
        }

        [HttpGet("getSuperHeros")]
        public async Task<ActionResult<List<SuperHeros>>> GetSuperHeros ()
        {
            List<SuperHeros>? superheros = await _superHeroRepository.GetSuperHeros();

            return Ok(superheros);
        }

        [HttpGet("getSuperHero/{id}")]
        [EnableRateLimiting("Fixed")]
        public async Task<ActionResult<SuperHeros>> GetSuperHero(int id)
        {
            SuperHeros? superhero = await _superHeroRepository.GetSuperHeroById(id);

            if (superhero == null)
            {
                return BadRequest("Hero not found.");
            }

            return Ok(superhero);
        }
    }
}
