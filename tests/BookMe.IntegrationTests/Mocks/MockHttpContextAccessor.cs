using Microsoft.AspNetCore.Http;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;

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
}