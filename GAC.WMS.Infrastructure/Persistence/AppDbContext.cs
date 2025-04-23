using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);          
            
        }
    }
}
