using Microsoft.EntityFrameworkCore;
using NetCoreWebAPI.Models;

namespace NetCoreWebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {   
        }

        public DbSet<SuperHeros> SuperHeros { get; set; }
        public DbSet<ExceptionLogger> ExceptionLogers { get; set; }
    }
}
