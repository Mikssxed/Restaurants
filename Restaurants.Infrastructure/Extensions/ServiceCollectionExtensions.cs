using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conntectionString = configuration.GetConnectionString("RestaurantsDb");
        services.AddDbContext<RestaurantDbContext>(options => options.UseSqlServer(conntectionString));
    }
}