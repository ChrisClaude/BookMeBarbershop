using BookMe.Infrastructure.Data;
using BookMe.IntegrationTests.Mocks;
using BookMe.IntegrationTests.TestData;
using Microsoft.Extensions.DependencyInjection;

namespace BookMe.IntegrationTests;

[Collection("Database collection")]
public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
        IAsyncDisposable
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
    }

    public async ValueTask DisposeAsync()
    {
        // Dispose resources
        _scope.Dispose();
        _bookMeContext.Dispose();
        GC.SuppressFinalize(this);

        await TestCDataCleanUp.CleanUpDatabaseAsync(_bookMeContext);
    }
}
