using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Repositories;

public class RestaurantsesRepository(RestaurantDbContext dbContext) : IRestaurantsRepository
{
    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        var restaurants = await dbContext.Restaurants.ToListAsync();
        return restaurants;
    }

    public Task<Restaurant?> GetByIdAsync(int id)
    {
        var restaurant = dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
        return restaurant;
    }
}