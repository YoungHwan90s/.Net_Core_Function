using Microsoft.EntityFrameworkCore;
using NetCoreWebAPI.Data;
using NetCoreWebAPI.Interfaces.Repositories;
using NetCoreWebAPI.Models;

namespace NetCoreWebAPI.Repositories
{
    public class SuperHeroRepository : ISuperHeroRepository
    {
        private readonly ApplicationDbContext _context;

        public SuperHeroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddSuperHero(List<SuperHeros> superHero)
        {
            _context.SuperHeros.AddRange(superHero);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<SuperHeros>?> GetSuperHeros()
        {
            return await _context.SuperHeros.ToListAsync();

            // Distributed cache example
            //try
            //{
            //    return await _cacheService.GetAsync<List<SuperHeros>>("superHeros", async () =>
            //    {
            //        List<SuperHeros>? superHeros = await _context.SuperHeros.ToListAsync();

            //        return superHeros;
            //    });
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }

        public async Task<SuperHeros?> GetSuperHeroById(int id)
        {
            return await _context.SuperHeros.FindAsync(id);
        }
    }
}
