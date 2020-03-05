using Xunit;

namespace DatingApp.DataAccess.Test
{
    /// <summary>
    /// The database collection fixture class.
    /// </summary>
    [CollectionDefinition("Database collection")]
    class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
