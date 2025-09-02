using Microsoft.Extensions.DependencyInjection;

namespace AgileBoard.Infrastructure
{
    public class TestDbConnection
    {
        public static void CheckConnection(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AgileBoardDbContext>();

                try
                {
                    dbContext.Database.EnsureCreated();
                    Console.WriteLine("Connection succeffully estabelished!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connection to database: {ex.Message}");
                }
            }
        }
    }
}
