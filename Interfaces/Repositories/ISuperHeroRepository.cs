using NetCoreWebAPI.Models;

namespace NetCoreWebAPI.Interfaces.Repositories
{
    public interface ISuperHeroRepository
    {
        Task<int> AddSuperHero(List<SuperHeros> superHero);
        Task<List<SuperHeros>> GetSuperHeros();
        Task<SuperHeros?> GetSuperHeroById(int id);

    }
}
