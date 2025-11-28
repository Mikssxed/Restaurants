using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Restaurants.Application.User;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Authorization.Requirements;
using Xunit;

namespace Restaurants.Infrastructure.Tests.Authorization.Requirements;

[TestSubject(typeof(CreatedMultipleRestaurantsRequirementHandler))]
public class CreatedMultipleRestaurantsRequirementHandlerTest
{

    [Fact]
    public async Task HandleRequirementAsync_UserHasCreatedMultipleRestaurants_ShouldSucceed()
    {
        // arrange
        var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

        var restaurants = new List<Restaurant>()
        {
            new()
            {
                OwnerId = "1"
            },
            new()
            {
                OwnerId = "1"
            },
            new()
            {
                OwnerId = "2"
            }
        };
        
        var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        restaurantsRepositoryMock.Setup(m => m.GetAllAsync()).ReturnsAsync(restaurants);
        
        var requirement = new CreatedMultipleRestaurantsRequirement(2);
        var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object);
        var authorizationContext = new AuthorizationHandlerContext(new[]{ requirement }, null, null);
        
        // act
        await handler.HandleAsync(authorizationContext);
        
        // assert
        authorizationContext.HasSucceeded.Should().BeTrue();
    }
    
    [Fact]
    public async Task HandleRequirementAsync_UserHasNotCreatedMultipleRestaurants_ShouldFail()
    {
        // arrange
        var currentUser = new CurrentUser("2", "test@test.com", [], null, null);
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

        var restaurants = new List<Restaurant>()
        {
            new()
            {
                OwnerId = "1"
            },
            new()
            {
                OwnerId = "1"
            },
            new()
            {
                OwnerId = "2"
            }
        };
        
        var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        restaurantsRepositoryMock.Setup(m => m.GetAllAsync()).ReturnsAsync(restaurants);
        
        var requirement = new CreatedMultipleRestaurantsRequirement(2);
        var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object);
        var authorizationContext = new AuthorizationHandlerContext(new[]{ requirement }, null, null);
        
        // act
        await handler.HandleAsync(authorizationContext);
        
        // assert
        authorizationContext.HasSucceeded.Should().BeFalse();
    }
}