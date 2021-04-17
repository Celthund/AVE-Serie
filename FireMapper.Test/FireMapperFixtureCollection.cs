using Xunit;

namespace FireMapper.Test
{
    [CollectionDefinition("FireStoreMapperFixture collection")]
    public class FireMapperFixtureCollection : ICollectionFixture<FireStoreMapperFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}