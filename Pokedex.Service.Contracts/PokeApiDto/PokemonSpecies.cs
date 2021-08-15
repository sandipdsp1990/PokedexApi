using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pokedex.Service.Contracts.PokeApiDto
{
    public class PokemonSpecies
    {
        [JsonProperty("flavor_text_entries")]
        public List<FlavorTextEntry> FlavorTextEntries { get; set; }

        [JsonProperty("habitat")]
        public PokemonSpeciesHabitat Habitat { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("is_legendary")]
        public bool IsLegendary { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class PokemonSpeciesLanguage
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class FlavorTextEntry
    {
        [JsonProperty("flavor_text")]
        public string FlavorText { get; set; }

        [JsonProperty("language")]
        public PokemonSpeciesLanguage Language { get; set; }

    }

    public class PokemonSpeciesHabitat
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    
}
