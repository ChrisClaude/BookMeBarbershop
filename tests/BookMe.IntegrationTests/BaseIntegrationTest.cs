using System;
using BookMe.Infrastructure.Data;
using BookMe.IntegrationTests.Mocks;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    protected readonly IntegrationTestWebAppFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly BookMeContext _bookMeContext;
    protected readonly MockHttpContextAccessor _mockHttpContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _scope = _factory.Services.CreateScope();
        _bookMeContext = _scope.ServiceProvider.GetRequiredService<BookMeContext>();
        _mockHttpContext = _factory.MockHttpContext;

        _mockHttpContext.SetCustomerUser();
    }


    public void Dispose()
    {
        _scope.Dispose();
        _bookMeContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
