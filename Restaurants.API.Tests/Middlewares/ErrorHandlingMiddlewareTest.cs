using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.API.Middlewares;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Xunit;

namespace Restaurants.API.Tests.Middlewares;

[TestSubject(typeof(ErrorHandlingMiddleware))]
public class ErrorHandlingMiddlewareTest
{

    [Fact]
    public async Task InvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
    {
        // arrange
        var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
        
        var context = new DefaultHttpContext();
        var nextDelegateMock = new Mock<RequestDelegate>();
        
        // act
        await middleware.InvokeAsync(context, nextDelegateMock.Object);
        
        // assert
        nextDelegateMock.Verify(next => next.Invoke(context), Times.Once);
    }
    
    [Fact]
    public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldSetStatusCode404()
    {
        // arrange
        var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
        
        var context = new DefaultHttpContext();
        var notFoundException = new NotFoundException(nameof(Restaurant), "1");
        
        // act
        await middleware.InvokeAsync(context, _ => throw notFoundException);
        
        // assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

    }
}