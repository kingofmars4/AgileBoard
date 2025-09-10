using AgileBoard.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Tests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
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

                services.AddDbContext<AgileBoardDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });
            });

            builder.UseEnvironment("Testing");
        }
    }
}