using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.User.Commands.UnassignUserRole;

public class UnassignUserRoleCommandHandler(
    ILogger<UnassignUserRoleCommandHandler> logger,
    UserManager<Domain.Entities.User> userManager,
    RoleManager<IdentityRole> roleManager) : IRequestHandler<UnassignUserRoleCommand>
{
    public async Task Handle(UnassignUserRoleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unassigning user role: {@Role}", request);
        var user = await userManager.FindByEmailAsync(request.UserEmail) ?? throw new NotFoundException(nameof(Domain.Entities.User), request.UserEmail);

        var hasRole = await userManager.IsInRoleAsync(user, request.RoleName);

        if (!hasRole) throw new NotFoundException(nameof(Domain.Entities.User), request.UserEmail);

        await userManager.RemoveFromRoleAsync(user, request.RoleName);
    }
}