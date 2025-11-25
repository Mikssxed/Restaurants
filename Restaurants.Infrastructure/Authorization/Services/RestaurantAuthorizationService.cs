using Microsoft.Extensions.Logging;
using Restaurants.Application.User;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Interfaces;

namespace Restaurants.Infrastructure.Authorization.Services;

public class RestaurantAuthorizationService(ILogger<RestaurantAuthorizationService> logger, IUserContext userContext) : IRestaurantAuthorizationService
{
    public bool Authorize(Restaurant restaurant, ResourceOperation resourceOperation)
    {
        var user = userContext.GetCurrentUser();
        logger.LogInformation("Authorizing user: {@UserId} for {@ResourceOperation} on restaurant: {@RestaurantId}", user?.Id, resourceOperation, restaurant.Id);

        if (resourceOperation == ResourceOperation.Read || resourceOperation == ResourceOperation.Create)
        {
            logger.LogInformation("Create/read operation - authorization granted");
            return true;
        }

        if (resourceOperation == ResourceOperation.Delete && user.IsInRole(UserRoles.Admin))
        {
            logger.LogInformation("Admin user, delete operation - authorization granted");
            return true;
        }

        if (resourceOperation == ResourceOperation.Delete || (resourceOperation == ResourceOperation.Update && user.Id == restaurant.OwnerId))
        {
            logger.LogInformation("Owner user, update/delete operation - authorization granted");
            return true;
        }

        return false;
    }
}