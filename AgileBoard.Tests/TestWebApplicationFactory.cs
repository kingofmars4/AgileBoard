using AgileBoard.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Tests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly string _testDatabaseName;

        public TestWebApplicationFactory()
        {
            _testDatabaseName = $"AgileBoardTestDb_{Guid.NewGuid():N}";
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var toRemove = new List<ServiceDescriptor>();
                
                foreach (var service in services)
                {
                    if (service.ServiceType == typeof(DbContextOptions<AgileBoardDbContext>) ||
                        service.ServiceType == typeof(AgileBoardDbContext) ||
                        service.ServiceType.ToString().Contains("EntityFramework") ||
                        service.ServiceType.ToString().Contains("SqlServer") ||
                        service.ImplementationType?.ToString().Contains("SqlServer") == true ||
                        service.ImplementationType?.ToString().Contains("EntityFramework") == true)
                    {
                        toRemove.Add(service);
                    }
                }

                foreach (var service in toRemove)
                {
                    services.Remove(service);
                }

                var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={_testDatabaseName};Trusted_Connection=true;MultipleActiveResultSets=true";

                services.AddDbContext<AgileBoardDbContext>(options =>
                {
                    options.UseSqlServer(connectionString)
                           .EnableSensitiveDataLogging()
                           .LogTo(Console.WriteLine, LogLevel.Information);
                });
                
                services.AddLogging(builder => builder.AddConsole());
            });

            builder.UseEnvironment("Testing");
        }

        public async Task InitializeDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AgileBoardDbContext>();
            
            await context.Database.EnsureCreatedAsync();
        }

        public async Task CleanupDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AgileBoardDbContext>();
            
            await context.Database.EnsureDeletedAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    using var scope = Services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AgileBoardDbContext>();
                    context.Database.EnsureDeleted();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error cleaning up test database: {ex.Message}");
                }
            }
            
            base.Dispose(disposing);
        }
    }
}