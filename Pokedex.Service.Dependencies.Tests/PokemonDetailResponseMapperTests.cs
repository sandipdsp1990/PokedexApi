using NUnit.Framework;
using Pokedex.Service.Contracts.PokeApiDto;
using Pokedex.Service.Contracts.ResponseDto;
using Pokedex.Service.Dependencies.Contracts;
using System.Linq;

namespace Pokedex.Service.Dependencies.Tests
{
    [TestFixture]
    public class PokemonDetailResponseMapperTests
    {
        private PokemonDetail result;
        private PokemonDetail expectedResult;
        private PokemonSpecies apiResponse;
        private PokemonDetailResponseMapper subject;

        [SetUp]
        public void Setup()
        {
            apiResponse = new PokemonSpecies
            {
                FlavorTextEntries = new System.Collections.Generic.List<FlavorTextEntry>()
               {
                   new FlavorTextEntry(){ FlavorText="English Flavour text",Language=new PokemonSpeciesLanguage(){ Name="en"} },
                   new FlavorTextEntry(){ FlavorText="French Flavour text",Language=new PokemonSpeciesLanguage(){ Name="fr"} }
               },
                Habitat = new PokemonSpeciesHabitat() { Name = "Habitat name" },
                Name = "pokemon Name",
                Id = 1,
                IsLegendary = true,
            };

            expectedResult = new PokemonDetail()
            {
                Description = "English Flavour text",
                Habitat = "Habitat name",
                IsLegendary = true,
                Name = "pokemon Name"
            };
            subject = new PokemonDetailResponseMapper();
        }

        [Test]
        public void MustImplementContract()
        {
            Assert.That(subject, Is.InstanceOf(typeof(IMapper<PokemonSpecies, PokemonDetail>)));
        }

        private void When()
        {
            result = subject.Map(apiResponse);
        }

        [Test]
        public void WhenMap_ThenMustAssignProperties()
        {
            When();

            //Then
            Assert.That(result.IsLegendary, Is.EqualTo(expectedResult.IsLegendary));
            Assert.That(result.Name, Is.EqualTo(expectedResult.Name));
            Assert.That(result.Habitat, Is.EqualTo(expectedResult.Habitat));
            Assert.That(result.Description, Is.EqualTo(expectedResult.Description));
        }

        [Test]
        public void WhenNewLineCharacters_Map_ThenMustEscapeDescription()
        {
            //Given
            apiResponse.FlavorTextEntries = new System.Collections.Generic.List<FlavorTextEntry>()
               {
                   new FlavorTextEntry(){ FlavorText="English\nFlavour text",Language=new PokemonSpeciesLanguage(){ Name="en"} },
                   new FlavorTextEntry(){ FlavorText="French Flavour text",Language=new PokemonSpeciesLanguage(){ Name="fr"} }
               };

            When();

            //Then
            Assert.That(result.IsLegendary, Is.EqualTo(expectedResult.IsLegendary));
            Assert.That(result.Name, Is.EqualTo(expectedResult.Name));
            Assert.That(result.Habitat, Is.EqualTo(expectedResult.Habitat));
            Assert.That(result.Description, Is.EqualTo(expectedResult.Description));
        }

        [Test]
        public void WhenFlavorTextEntriesNOtInEnglish_Map_ThenMustAssignProperties()
        {
            //Given
            apiResponse.FlavorTextEntries = new System.Collections.Generic.List<FlavorTextEntry>()
               {
                   new FlavorTextEntry(){ FlavorText="Korean Flavour text",Language=new PokemonSpeciesLanguage(){ Name="ko"} },
                   new FlavorTextEntry(){ FlavorText="French Flavour text",Language=new PokemonSpeciesLanguage(){ Name="fr"} }
               };

            result = subject.Map(apiResponse);

            //Then
            Assert.That(result.IsLegendary, Is.EqualTo(expectedResult.IsLegendary));
            Assert.That(result.Name, Is.EqualTo(expectedResult.Name));
            Assert.That(result.Habitat, Is.EqualTo(expectedResult.Habitat));
            Assert.That(result.Description, Is.EqualTo(null));
        }
    }
}