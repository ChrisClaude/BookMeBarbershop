using BookMe.Application.Common.Dtos;
using BookMe.Infrastructure.Data;

namespace BookMe.IntegrationTests;

[Collection("Database collection")]
public abstract class BaseIntegrationTest : IClassFixture<AspireIntegrationTestFixture>, IAsyncDisposable
{
    private const string TEST_USER_EMAIL_HEADER = "X-Test-User-Email";

    protected readonly HttpClient _client;
    protected readonly BookMeContext _bookMeContext;

    protected BaseIntegrationTest(AspireIntegrationTestFixture factory)
    {
        _client = factory.CreateApiClient();
        _bookMeContext = factory.CreateDbContext();
    }

    /// <summary>
    /// Authenticates subsequent requests as the given user (or clears auth when null),
    /// via the Testing-only header auth handler. See BookMeAPI/Authentication/TestAuthHandler.cs.
    /// </summary>
    protected void SetUser(UserDto? user)
    {
        _client.DefaultRequestHeaders.Remove(TEST_USER_EMAIL_HEADER);

        if (user is not null)
        {
            _client.DefaultRequestHeaders.Add(TEST_USER_EMAIL_HEADER, user.Email);
        }
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        _bookMeContext.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}