using Microsoft.AspNetCore.Mvc;

namespace Pokedex.Api.Controllers
{
    public class PokemonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
