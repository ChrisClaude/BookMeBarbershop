using Xunit;

namespace BookMe.IntegrationTests;

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<IntegrationTestWebAppFactory>
{
    // This class has no code, and is never created.
    // Its purpose is to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
