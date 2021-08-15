using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using Pokedex.Service.Contracts;
using Pokedex.Service.Contracts.PokeApiDto;
using Pokedex.Service.Contracts.ResponseDto;
using Pokedex.Service.Dependencies.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex.Service.Tests
{
    [TestFixture]
    public class PokemonTranslatedDetailTests
    {
        private Mock<HttpMessageHandler> httpHandlerMock;
        private Mock<HttpMessageHandler> translatedHttpHandlerMock;

        private PokemonSpecies pokemonSpeciesHttpResponseData;
        private FunTranslatedResponse translatedHttpResponseData;

        private HttpResponseMessage speciesResponseMessageData;
        private HttpResponseMessage translatedResponseMessageData;

        private Mock<IHttpClientFactory> mockFactory;
        private Mock<IMapper<PokemonSpecies, PokemonDetail>> mapper;
        private PokeApiService subject;
        private ServiceResult<PokemonDetail> expectedServiceResponse;
        private ServiceResult<PokemonDetail> expectedPokeApiServiceResponse;
        private ServiceResult<PokemonDetail> receivedServiceResponse;
        string pokemonName = "mewtwo";


        [SetUp]
        public void Setup()
        {
            mockFactory = new Mock<IHttpClientFactory>();
            pokemonSpeciesHttpResponseData = new PokemonSpecies
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

            translatedHttpResponseData = new FunTranslatedResponse()
            {
                Success = new FunTranslationSuccess() { Total = 1 },
                Contents = new FunTranslationContents() { Text = "English Flavour text", Translated = "Translated Flavour text", Translation = "shakespeare" }
            };

            expectedPokeApiServiceResponse = new SuccessServiceResult<PokemonDetail>(new PokemonDetail()
            {
                Description = "English Flavour text",
                Habitat = "Habitat name",
                IsLegendary = true,
                Name = "pokemon Name"
            });

            expectedServiceResponse = new SuccessServiceResult<PokemonDetail>(new PokemonDetail()
            {
                Description = "Translated Flavour text",
                Habitat = "Habitat name",
                IsLegendary = true,
                Name = "pokemon Name"
            });


            speciesResponseMessageData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(pokemonSpeciesHttpResponseData))
            };

            translatedResponseMessageData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(translatedHttpResponseData))
            };

            mapper = new Mock<IMapper<PokemonSpecies, PokemonDetail>>();
            httpHandlerMock = new Mock<HttpMessageHandler>();
            httpHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
                  )
               .ReturnsAsync(speciesResponseMessageData);

            translatedHttpHandlerMock = new Mock<HttpMessageHandler>();
            translatedHttpHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
                  )
               .ReturnsAsync(translatedResponseMessageData);

            var speciesClient = new HttpClient(httpHandlerMock.Object) { BaseAddress = new Uri("https://testapi.com") };

            mapper.Setup(x => x.Map(It.IsAny<PokemonSpecies>())).Returns(expectedPokeApiServiceResponse.ResultObject);

            var tranlationClient = new HttpClient(translatedHttpHandlerMock.Object) { BaseAddress = new Uri("https://testapi.com") }; ;
            mockFactory.Setup(_ => _.CreateClient("PokeApi")).Returns(speciesClient);
            mockFactory.Setup(_ => _.CreateClient("FunTranslationsApi")).Returns(tranlationClient);
            subject = new PokeApiService(mockFactory.Object, mapper.Object);
        }

        private void When()
        {
            receivedServiceResponse = subject.GetPokemonTranslatedDetail(pokemonName).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenMustCallPokeApiGetCall()
        {
            When();

            // Then

            httpHandlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());

            translatedHttpHandlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public void ThenMustCallMapper()
        {
            When();

            // Then

            mapper.Verify(x => x.Map(It.IsAny<PokemonSpecies>()), Times.Once);
        }

        [Test]
        public void WhenIsLegendary_GetPokemonTranslatedDetail_ThenMustCallFunTranlationWithYodaGetCall()
        {
            When();

            // Then

            translatedHttpHandlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsolutePath.Contains("yoda.json")),
               ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public void WhenHabitatCave_GetPokemonTranslatedDetail_ThenMustCallFunTranlationWithYodaGetCall()
        {
            //Given
            expectedPokeApiServiceResponse.ResultObject.Habitat = "cave";
            expectedPokeApiServiceResponse.ResultObject.IsLegendary = false;

            When();

            // Then

            translatedHttpHandlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsolutePath.Contains("yoda.json")),
               ItExpr.IsAny<CancellationToken>());
        }


        [Test]
        public void WhenHabitatIsNotCaveAndNotLegendary_GetPokemonTranslatedDetail_ThenMustCallFunTranlationWithShakespeareGetCall()
        {
            //Given
            expectedPokeApiServiceResponse.ResultObject.Habitat = "urban";
            expectedPokeApiServiceResponse.ResultObject.IsLegendary = false;

            When();

            // Then

            translatedHttpHandlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsolutePath.Contains("shakespeare.json")),
               ItExpr.IsAny<CancellationToken>());
        }


        [Test]
        public void GivenSpeciesServiceResonseIsOK__GetPokemonTranslatedDetail_ThenMustReturnExpectedResponse()
        {
            When();

            // Then

            Assert.NotNull(receivedServiceResponse);
            Assert.That(receivedServiceResponse.Success, Is.EqualTo(true));
            Assert.That(receivedServiceResponse.ResultObject.IsLegendary, Is.EqualTo(expectedServiceResponse.ResultObject.IsLegendary));
            Assert.That(receivedServiceResponse.ResultObject.Name, Is.EqualTo(expectedServiceResponse.ResultObject.Name));
            Assert.That(receivedServiceResponse.ResultObject.Habitat, Is.EqualTo(expectedServiceResponse.ResultObject.Habitat));
            Assert.That(receivedServiceResponse.ResultObject.Description, Is.EqualTo(expectedServiceResponse.ResultObject.Description));
        }

        [Test]
        public void GivenServiceReturnNotOK__GetPokemonTranslatedDetail_ThenMustReturnWithoutTranslation()
        {
            //Given

            translatedResponseMessageData.StatusCode = HttpStatusCode.InternalServerError;

            When();

            // Then

            Assert.NotNull(receivedServiceResponse);
            Assert.That(receivedServiceResponse.Success, Is.EqualTo(true));
            Assert.That(receivedServiceResponse.ResultObject.IsLegendary, Is.EqualTo(expectedServiceResponse.ResultObject.IsLegendary));
            Assert.That(receivedServiceResponse.ResultObject.Name, Is.EqualTo(expectedServiceResponse.ResultObject.Name));
            Assert.That(receivedServiceResponse.ResultObject.Habitat, Is.EqualTo(expectedServiceResponse.ResultObject.Habitat));
            // when service return unsucessfull then use original description.
            Assert.That(receivedServiceResponse.ResultObject.Description, Is.EqualTo(expectedPokeApiServiceResponse.ResultObject.Description));
        }

        [Test]
        public void GivenServiceReturnOKWithErrors__GetPokemonTranslatedDetail_ThenMustReturnWithoutTranslation()
        {
            //Given
            translatedHttpResponseData.Error = new FunTranslationError() { Code = 500, Message = "Test Error" };
            translatedResponseMessageData.Content = new StringContent(JsonConvert.SerializeObject(translatedHttpResponseData));

            When();

            // Then

            Assert.NotNull(receivedServiceResponse);
            Assert.That(receivedServiceResponse.Success, Is.EqualTo(true));
            Assert.That(receivedServiceResponse.ResultObject.IsLegendary, Is.EqualTo(expectedServiceResponse.ResultObject.IsLegendary));
            Assert.That(receivedServiceResponse.ResultObject.Name, Is.EqualTo(expectedServiceResponse.ResultObject.Name));
            Assert.That(receivedServiceResponse.ResultObject.Habitat, Is.EqualTo(expectedServiceResponse.ResultObject.Habitat));
            // when service return unsucessfull then use original description.
            Assert.That(receivedServiceResponse.ResultObject.Description, Is.EqualTo(expectedPokeApiServiceResponse.ResultObject.Description));
        }

        [Test]
        public void GivenServiceResonseIsNotOK__GetPokemonTranslatedDetail_ThenMustReturnFalseResponse()
        {
            //Given
            speciesResponseMessageData.StatusCode = HttpStatusCode.NotFound;

            When();

            // Then

            Assert.IsNull(receivedServiceResponse.ResultObject);
            Assert.That(receivedServiceResponse.Success, Is.EqualTo(false));
        }

        [Test]
        public void GivenServiceThrowsException__GetPokemonTranslatedDetail_ThenMustReturnFalseResponse()
        {
            httpHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.IsAny<HttpRequestMessage>(),
                 ItExpr.IsAny<CancellationToken>()
                 )
             .Throws(new HttpRequestException());

            When();

            // Then

            Assert.IsNull(receivedServiceResponse.ResultObject);
            Assert.That(receivedServiceResponse.Success, Is.EqualTo(false));
            Assert.That(receivedServiceResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
        }

        [Test]
        public void GivenTranslatedHttpHandlerThrowsException__GetPokemonTranslatedDetail_ThenMustReturnSuccessResponse()
        {
            translatedHttpHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.IsAny<HttpRequestMessage>(),
                 ItExpr.IsAny<CancellationToken>()
                 )
             .Throws(new HttpRequestException());

            When();

            // Then

            Assert.That(receivedServiceResponse.Success, Is.EqualTo(true));
            Assert.That(receivedServiceResponse.ResultObject.IsLegendary, Is.EqualTo(expectedServiceResponse.ResultObject.IsLegendary));
            Assert.That(receivedServiceResponse.ResultObject.Name, Is.EqualTo(expectedServiceResponse.ResultObject.Name));
            Assert.That(receivedServiceResponse.ResultObject.Habitat, Is.EqualTo(expectedServiceResponse.ResultObject.Habitat));
            Assert.That(receivedServiceResponse.ResultObject.Description, Is.EqualTo(expectedPokeApiServiceResponse.ResultObject.Description));
        }
    }
}
