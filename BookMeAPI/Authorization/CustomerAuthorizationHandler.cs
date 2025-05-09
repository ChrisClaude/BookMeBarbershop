using System;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BookMeAPI.Authorization;

public class CustomerRequirement : IAuthorizationRequirement
{

}

public class CustomerAuthorizationHandler : AuthorizationHandler<CustomerRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomerAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomerRequirement requirement)
    {
        if (_httpContextAccessor.HttpContext?.Items[Constant.HTTP_CONTEXT_USER_ITEM_KEY] is not UserDto user)
        {
            return Task.CompletedTask;
        }

        if (user.IsCustomer)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
