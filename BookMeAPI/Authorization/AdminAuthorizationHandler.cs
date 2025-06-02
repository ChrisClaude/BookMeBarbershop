using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace BookMeAPI.Authorization;

public class AdminRequirement : IAuthorizationRequirement
{
    public bool IsAdmin { get; set; }

    public AdminRequirement(bool isAdmin)
    {
        IsAdmin = isAdmin;
    }
}

public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor =
            httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement
    )
    {
        if (
            _httpContextAccessor.HttpContext?.Items[Constant.HTTP_CONTEXT_USER_ITEM_KEY]
            is not UserDto user
        )
        {
            return Task.CompletedTask;
        }

        if (requirement.IsAdmin && user.IsAdmin())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
