using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Restaurants.API.Controllers;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Seeders;
using Xunit;

namespace Restaurants.API.Tests.Controllers;

[TestSubject(typeof(RestaurantsController))]
public class RestaurantsControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock = new();

    public RestaurantsControllerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(b =>
        {
            b.ConfigureTestServices(services =>
            {
                // Add fake policy evaluator for auth
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                
                // Replace seeder with mock to prevent database seeding
                services.RemoveAll<IRestaurantSeeder>();
                services.AddScoped<IRestaurantSeeder>(_ => new Mock<IRestaurantSeeder>().Object);
                
                // Replace repository with mock
                services.RemoveAll<IRestaurantsRepository>();
                services.AddScoped<IRestaurantsRepository>(_ => _restaurantsRepositoryMock.Object);
            });
        });
    }
    [Fact]
    public async Task GetById_ForNonExistingId_ShouldReturn404NotFound()
    {
        // arrange

        var id = 1123;

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Restaurant?)null);

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/restaurants/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ForExistingId_ShouldReturn200Ok()
    {
        // arrange

        var id = 99;

        var restaurant = new Restaurant()
        {
            Id = id,
            Name = "Test",
            Description = "Test description"
        };

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(restaurant);

        var client = _factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/restaurants/{id}");
        var restaurantDto = await response.Content.ReadFromJsonAsync<RestaurantDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantDto.Should().NotBeNull();
        restaurantDto.Name.Should().Be("Test");
        restaurantDto.Description.Should().Be("Test description");
    }

    [Fact]
    public async Task GetAll_ForValidRequest_Returns200Ok()
    {
        // arrange
        var client = _factory.CreateClient();

        // act
        var result = await client.GetAsync("/api/restaurants?pageNumber=1&pageSize=10");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

    }

    [Fact]
    public async Task GetAll_ForInvalidRequest_Returns400BadRequest()
    {
        // arrange
        var client = _factory.CreateClient();

        // act
        var result = await client.GetAsync("/api/restaurants");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

    }
}