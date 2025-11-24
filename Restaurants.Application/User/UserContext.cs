using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Restaurants.Application.User;

public interface IUserContext
{
    CurrentUser GetCurrentUser();
}

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public CurrentUser GetCurrentUser()
    {
        var user = accessor?.HttpContext?.User;

        if (user == null) throw new InvalidOperationException("User context is not present.");

        if (user.Identity == null || !user.Identity.IsAuthenticated) throw new InvalidOperationException("User is not authenticated.");

        var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var email = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
        var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role)!.Select(c => c.Value);

        return new CurrentUser(userId, email, roles);
    }
}