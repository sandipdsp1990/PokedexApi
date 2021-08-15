using Newtonsoft.Json;
using Pokedex.Service.Contracts;
using Pokedex.Service.Contracts.PokeApiDto;
using Pokedex.Service.Contracts.ResponseDto;
using Pokedex.Service.Dependencies.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pokedex.Service
{
    public class PokeApiService : IPokeApiService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IMapper<PokemonSpecies, PokemonDetail> pokemonDetailMapper;


        public PokeApiService(IHttpClientFactory clientFactory, IMapper<PokemonSpecies, PokemonDetail> pokemonDetailMapper)
        {
            this.clientFactory = clientFactory;
            this.pokemonDetailMapper = pokemonDetailMapper;
        }

        public async Task<ServiceResult<PokemonDetail>> GetPokemonDetail(string name)
        {
            var pokeClient = clientFactory.CreateClient("PokeApi");

            try
            {
                var response = await pokeClient.GetAsync($"pokemon-species/{WebUtility.UrlEncode(name)}");
                if (response.IsSuccessStatusCode)
                {
                    var pokemonSpecies = JsonConvert.DeserializeObject<PokemonSpecies>(await response.Content.ReadAsStringAsync());
                    var responseDto = pokemonDetailMapper.Map(pokemonSpecies);
                    return new SuccessServiceResult<PokemonDetail>(responseDto);
                }
                else
                {
                    return new FalseServiceResult<PokemonDetail>(HttpStatusCode.NotFound, "Oops! Can't find Pokemon!", null);
                }
            }
            catch (HttpRequestException)
            {
                return new FalseServiceResult<PokemonDetail>(HttpStatusCode.ServiceUnavailable, "Oops! Service Unavailable!", null);
            }

        }

        public async Task<ServiceResult<PokemonDetail>> GetPokemonTranslatedDetail(string name)
        {
            var pokemonDetailResponse = await GetPokemonDetail(name);
            if (pokemonDetailResponse.Success)
            {
                var pokemonDetail = pokemonDetailResponse.ResultObject;

                pokemonDetail.Description = await GetTranslatedText(pokemonDetail.Description, (pokemonDetail.Habitat != null && pokemonDetail.Habitat.Equals("cave", StringComparison.OrdinalIgnoreCase) || pokemonDetail.IsLegendary));
                return new SuccessServiceResult<PokemonDetail>(pokemonDetail);
            }
            else
            {
                return new FalseServiceResult<PokemonDetail>(pokemonDetailResponse.HttpStatusCode, pokemonDetailResponse.Message, pokemonDetailResponse.ResultObject);
            }

        }

        private async Task<string> GetTranslatedText(string originalText, bool isYodaTranslation)
        {
            var funTranslationClient = clientFactory.CreateClient("FunTranslationsApi");
            HttpResponseMessage response;
            var requestUrl = $"{(isYodaTranslation ? "yoda" : "shakespeare")}.json?text={WebUtility.UrlEncode(originalText)}";

            try
            {
                response = await funTranslationClient.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var translatedText = JsonConvert.DeserializeObject<FunTranslatedResponse>(await response.Content.ReadAsStringAsync());
                    if (translatedText.Success.Total > 0 && translatedText.Error == null)
                    {
                        return translatedText.Contents.Translated;
                    }
                }
            }
            catch (Exception)
            {
                return originalText;
            }

            return originalText;
        }
    }
}

