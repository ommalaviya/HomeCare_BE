using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.HomeCare.Data
{
    // EF Core design-time factory
    public class HomeCareDbContextFactory : IDesignTimeDbContextFactory<HomeCareDbContext>
    {
        public HomeCareDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HomeCareDbContext>();

            // PostgreSQL connection string
            optionsBuilder.UseNpgsql("Host=localhost;Database=localhomecare;Username=localhomecare;Password=Admin@123");

            return new HomeCareDbContext(optionsBuilder.Options);
        }
    }
}