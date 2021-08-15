using Pokedex.Service.Contracts.ResponseDto;
using System.Threading.Tasks;

namespace Pokedex.Service.Contracts
{
    public interface IPokeApiService
    {
        Task<ServiceResult<PokemonDetail>> GetPokemonDetail(string name);
        Task<ServiceResult<PokemonDetail>> GetPokemonTranslatedDetail(string name);
    }
}
