using System;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
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
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
        if (context.Resource is not DefaultHttpContext defaultHttpContext ||
                defaultHttpContext.Items[Constant.HTTP_CONTEXT_USER_ITEM_KEY] is not UserDto user)
        {
            return Task.CompletedTask;
        }

        if (requirement.IsAdmin && user.IsAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
