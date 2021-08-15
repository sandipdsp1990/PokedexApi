using Newtonsoft.Json;

namespace Pokedex.Service.Contracts.ResponseDto
{
    public class PokemonDetail
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("habitat")]
        public string Habitat { get; set; }

        [JsonProperty("isLegendary")]
        public bool IsLegendary { get; set; }

    }
}
