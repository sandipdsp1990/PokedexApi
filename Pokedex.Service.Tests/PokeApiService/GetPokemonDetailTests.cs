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
    public class GetPokemonDetailTests
    {
        private Mock<HttpMessageHandler> httpHandlerMock;
        private PokemonSpecies httpResponseData;
        private HttpResponseMessage responseMessageData;
        private  Mock<IHttpClientFactory> mockFactory;
        private Mock<IMapper<PokemonSpecies, PokemonDetail>> mapper;
        private  PokeApiService subject;
        private ServiceResult<PokemonDetail> expectedServiceResponse;
        private ServiceResult<PokemonDetail> receivedServiceResponse;
        string pokemonName = "mewtwo";


        [SetUp]
        public void Setup()
        {
            mockFactory = new Mock<IHttpClientFactory>();
            httpResponseData = new PokemonSpecies
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

            expectedServiceResponse = new SuccessServiceResult<PokemonDetail>(new PokemonDetail()
            {
                Description = "English Flavour text",
                Habitat = "Habitat name",
                IsLegendary = true,
                Name = "pokemon Name"
            });

            responseMessageData = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(httpResponseData))
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
               .ReturnsAsync(responseMessageData);

            var client = new HttpClient(httpHandlerMock.Object);
            client.BaseAddress =new Uri("https://test.com");

            mapper.Setup(x => x.Map(It.IsAny<PokemonSpecies>())).Returns(expectedServiceResponse.ResultObject);
            mockFactory.Setup(_ => _.CreateClient("PokeApi")).Returns(client);
            subject = new PokeApiService(mockFactory.Object, mapper.Object);
        }

        private void When()
        {
            receivedServiceResponse = subject.GetPokemonDetail(pokemonName).GetAwaiter().GetResult();
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
        }

        [Test]
        public void ThenMustCallMapper()
        {
            When();

            // Then

            mapper.Verify(x => x.Map(It.IsAny<PokemonSpecies>()), Times.Once);
        }

        [Test]
        public void GivenServiceResonseIsOK__GetPokemonDetail_ThenMustReturnExpectedResponse()
        {
            When();

            // Then

            Assert.NotNull(receivedServiceResponse);
            Assert.That(receivedServiceResponse.ResultObject, Is.EqualTo(expectedServiceResponse.ResultObject));
            Assert.That(receivedServiceResponse.Success, Is.EqualTo(true));

        }

        [Test]
        public void GivenServiceResonseIsNotOK__GetPokemonDetail_ThenMustReturnFalseResponse()
        {
            //Given
            responseMessageData.StatusCode = HttpStatusCode.NotFound;

            When();

            // Then

            Assert.IsNull(receivedServiceResponse.ResultObject);
            Assert.That(receivedServiceResponse.Success, Is.EqualTo(false));
        }

        [Test]
        public void GivenServiceResonseThrowsException__GetPokemonDetail_ThenMustReturnFalseResponse()
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
    }
}