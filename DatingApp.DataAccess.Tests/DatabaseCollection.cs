using DatingApp.DataAccess.Test;
using Xunit;

namespace DatingApp.DataAccess.Tests
{
    [CollectionDefinition("Database collection")]
    class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
}
