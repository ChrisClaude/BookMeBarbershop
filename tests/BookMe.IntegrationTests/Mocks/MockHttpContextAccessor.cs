using Microsoft.AspNetCore.Http;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Common.Dtos.Users;

namespace BookMe.IntegrationTests.Mocks;

public class MockHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; } = new DefaultHttpContext();

    public MockHttpContextAccessor()
    {
        HttpContext = new DefaultHttpContext();
    }

    public void SetUser(UserDto user)
    {
        if (HttpContext != null)
        {
            HttpContext.Items[Constant.HTTP_CONTEXT_USER_ITEM_KEY] = user;
        }
    }

    public void SetCustomerUser()
    {
        SetUser(CreateTestUser(RoleName.CUSTOMER, DefaultRoles.CustomerId));
    }

    public void SetAdminUser()
    {
        SetUser(CreateTestUser(RoleName.ADMIN, DefaultRoles.AdminId));
    }

    private UserDto CreateTestUser(string roleName, Guid roleId)
    {
        return new UserDto
        {
            Id = Guid.NewGuid(),
            Email = $"john.{roleName.ToLower()}@test.com",
            Name = "John",
            Surname = roleName,
            PhoneNumber = "1234567890",
            Roles = new List<UserRoleDto>
            {
                new UserRoleDto
                {
                    Role = new RoleDto
                    {
                        Name = roleName
                    }
                }
            }
        };
    }
}