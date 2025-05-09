using System;
using BookMe.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{

    protected readonly IServiceScope _scope;
    protected readonly BookMeContext _bookMeContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _bookMeContext = _scope.ServiceProvider.GetRequiredService<BookMeContext>();
    }


    public void Dispose()
    {
        _scope.Dispose();
        _bookMeContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
