using NUnit.Framework;
using Pokedex.Service.Contracts;

namespace Pokedex.Service.Tests
{
    [TestFixture]
    public class PokeApiServiceTests
    {
        [TestFixture]
        public class ServiceTests
        {
            private PokeApiService subject;

            [SetUp]
            public void Setup()
            {
                subject = new PokeApiService(null, null);
            }

            [Test]
            public void ImplementsContract()
            {
                Assert.That(subject, Is.InstanceOf(typeof(IPokeApiService)));
            }
        }
    }
}