using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pokedex.Api.Controllers;
using Pokedex.Service.Contracts;
using Pokedex.Service.Contracts.ResponseDto;
using System.Net;

namespace Pokedex.Service.Tests
{
    [TestFixture]
    public class GetPokemonDetailTests
    {
        ServiceResult<PokemonDetail> expectedResult;
        PokemonController subject;
        Mock<IPokeApiService> pokeApiService;
        string pokemonName = "mewtwo";
        private IActionResult response;


        [SetUp]
        public void Setup()
        {
            expectedResult = new SuccessServiceResult<PokemonDetail>(new PokemonDetail()
            {
                Description = "English Flavour text",
                Habitat = "Habitat name",
                IsLegendary = true,
                Name = "pokemon Name"
            });

            pokeApiService = new Mock<IPokeApiService>();
            pokeApiService.Setup(x => x.GetPokemonDetail(pokemonName)).ReturnsAsync(expectedResult);
            subject = new PokemonController(pokeApiService.Object);
        }

        private void When()
        {
            response = subject.PokemonDetails(pokemonName).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenMustCallPokeApiServiceGetPokemonDetailsl()
        {
            When();

            // Then
            pokeApiService.Verify(x => x.GetPokemonDetail(pokemonName), Times.Once);

        }


        [Test]
        public void GivenServiceReturnsTrue__PokemonDetails_ThenMustReturnOk()
        {
            When();

            // Then

            Assert.That(response.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        }

        [Test]
        public void GivenServiceReturnsTrue__PokemonDetails_ThenMustReturnResultsFromService()
        {
            When();

            // Then

            var result = response as OkObjectResult;
            var responseResult = result.Value as PokemonDetail;
            Assert.IsNotNull(responseResult);
            AssertResponse(responseResult);
        }

        [Test]
        public void GivenServiceResonseIsNotOK__GetPokemonDetail_ThenMustReturnNotFoundResponse()
        {
            //Given
            pokeApiService.Setup(x => x.GetPokemonDetail(pokemonName)).ReturnsAsync(new FalseServiceResult<PokemonDetail>(HttpStatusCode.NotFound, "test", null));

            When();

            // Then

            Assert.That(response.GetType(), Is.EqualTo(typeof(ContentResult)));
            var responseResult = response as ContentResult;
            Assert.IsNotNull(responseResult);
            Assert.That(responseResult.Content, Is.EqualTo("test"));
            Assert.That(responseResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        private void AssertResponse(PokemonDetail responseResult)
        {
            Assert.That(responseResult.Name, Is.EqualTo(expectedResult.ResultObject.Name));
            Assert.That(responseResult.Description, Is.EqualTo(expectedResult.ResultObject.Description));
            Assert.That(responseResult.IsLegendary, Is.EqualTo(expectedResult.ResultObject.IsLegendary));
            Assert.That(responseResult.Habitat, Is.EqualTo(expectedResult.ResultObject.Habitat));

        }
    }
}