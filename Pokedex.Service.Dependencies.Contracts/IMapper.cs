namespace Pokedex.Service.Dependencies.Contracts
{
    public interface IMapper<in TIn, out TOut>
    {
        TOut Map(TIn request);
    }
}
