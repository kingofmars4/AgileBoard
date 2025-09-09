using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AgileBoard.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AgileBoardDbContext>
    {
        public AgileBoardDbContext CreateDbContext(string[] args)
        {

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\AgileBoard.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<AgileBoardDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);

            return new AgileBoardDbContext(builder.Options);
        }
    }
}
