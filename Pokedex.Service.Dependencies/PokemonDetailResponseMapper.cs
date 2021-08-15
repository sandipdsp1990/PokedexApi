using Pokedex.Service.Contracts.PokeApiDto;
using Pokedex.Service.Contracts.ResponseDto;
using Pokedex.Service.Dependencies.Contracts;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pokedex.Service.Dependencies
{
    public class PokemonDetailResponseMapper : IMapper<PokemonSpecies, PokemonDetail>
    {
        public PokemonDetail Map(PokemonSpecies pokemonSpecies)
        {
            return new PokemonDetail
            {
                Description = pokemonSpecies.FlavorTextEntries.FirstOrDefault(dc => dc.Language.Name == "en") != null ? Regex.Replace(pokemonSpecies.FlavorTextEntries.FirstOrDefault(dc => dc.Language.Name == "en").FlavorText, @"\r\n?|\t|\n|\r|\f", " ") : null,
                Habitat = pokemonSpecies.Habitat?.Name,
                IsLegendary = pokemonSpecies.IsLegendary,
                Name = pokemonSpecies.Name,
            };
        }
    }
}
