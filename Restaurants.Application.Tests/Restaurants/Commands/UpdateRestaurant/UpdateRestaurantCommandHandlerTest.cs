using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using Xunit;

namespace Restaurants.Application.Tests.Restaurants.Commands.UpdateRestaurant;

[TestSubject(typeof(UpdateRestaurantCommandHandler))]
public class UpdateRestaurantCommandHandlerTest
{
    private readonly Mock<ILogger<UpdateRestaurantCommandHandler>> _loggerMock = new();
    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRestaurantAuthorizationService> _restaurantAuthorizationServiceMock = new();

    private readonly UpdateRestaurantCommandHandler _handler;

    public UpdateRestaurantCommandHandlerTest()
    {
        _handler = new UpdateRestaurantCommandHandler(_loggerMock.Object, _mapperMock.Object, _restaurantsRepositoryMock.Object, _restaurantAuthorizationServiceMock.Object);
    }
    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateRestaurant()
    {
        // arrange
        var restaurantId = 1;
        var command = new UpdateRestaurantCommand()
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Description = "Test Restaurant",
            HasDelivery = true,
        };

        var restaurant = new Restaurant()
        {
            Id = restaurantId,
            Name = "Old Name",
            Description = "Old Description",
            HasDelivery = false,
        };
        
        _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
        
        _restaurantAuthorizationServiceMock.Setup(r => r.Authorize(restaurant, ResourceOperation.Update)).Returns(true);
        
        // act
        await _handler.Handle(command, CancellationToken.None);
        
        // assert
        _restaurantsRepositoryMock.Verify(r => r.Update(restaurant), Times.Once);
        _mapperMock.Verify(m => m.Map(command, restaurant), Times.Once);
    }
    
    [Fact]
    public async Task Handle_WithNonExistingRestaurant_ShouldThrowNotFoundException()
    {
        // arrange
        var restaurantId = 1;
        var command = new UpdateRestaurantCommand()
        {
            Id = restaurantId,
            Name = "Test Restaurant",
            Description = "Test Restaurant",
            HasDelivery = true,
        };

        _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant)null);
        
        // act
        
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        // assert
        await act.Should().ThrowAsync<NotFoundException>();

    }
}