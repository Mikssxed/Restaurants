using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants;

public class RestaurantsService(IRestaurantsRepository restaurantsRepository, ILogger<RestaurantsService> logger) : IRestaurantsService
{
    public async Task<IEnumerable<Restaurant>> GetAllRestaurants()
    {
        logger.LogInformation("Getting all restaurants");
        var restaurants =  await restaurantsRepository.GetAllAsync();
        return restaurants;
    }

    public Task<Restaurant?> GetById(int id)
    {
        logger.LogInformation("Getting restaurant by id: {Id}", id);
        var restaurant = restaurantsRepository.GetByIdAsync(id);
        return restaurant;
    }
}