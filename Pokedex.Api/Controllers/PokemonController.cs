using Microsoft.AspNetCore.Mvc;
using Pokedex.Service.Contracts;
using Pokedex.Service.Contracts.ResponseDto;
using System.Net;
using System.Threading.Tasks;

namespace Pokedex.Api.Controllers
{
    public class PokemonController : Controller
    {
        #region Variable Declaration

        private readonly IPokeApiService pokeApiService;

        #endregion

        public PokemonController(IPokeApiService pokeApiService)
        {
            this.pokeApiService = pokeApiService;
        }

        [HttpGet]
        [Route("pokemon/{name}")]
        [ProducesResponseType(typeof(PokemonDetail), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> PokemonDetails(string name)
        {
            var serviceResult = await pokeApiService.GetPokemonDetail(name);

            if (serviceResult.Success)
            {
                return new OkObjectResult(serviceResult.ResultObject);
            }
            else
            {
                return new ContentResult
                {
                    StatusCode = (int)serviceResult.HttpStatusCode,
                    Content = serviceResult.Message
                };
            }
        }

        [HttpGet]
        [Route("pokemon/translated/{name}")]
        [ProducesResponseType(typeof(PokemonDetail), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> PokemonTranslatedDetail(string name)
        {
            var serviceResult = await pokeApiService.GetPokemonTranslatedDetail(name);

            if (serviceResult.Success)
            {
                return new OkObjectResult(serviceResult.ResultObject);
            }
            else
            {
                return new ContentResult
                {
                    StatusCode = (int)serviceResult.HttpStatusCode,
                    Content = serviceResult.Message
                };
            }
        }

    }
}
