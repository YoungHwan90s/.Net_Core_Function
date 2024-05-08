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

        public async Task<List<SuperHeros>> GetSuperHeros()
        {
            return await _context.SuperHeros.ToListAsync();
        }

        public async Task<SuperHeros?> GetSuperHeroById(int id)
        {
            return await _context.SuperHeros.FindAsync(id);
        }
    }
}
