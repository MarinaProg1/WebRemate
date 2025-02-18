using Microsoft.AspNetCore.Mvc;
using WebRemate.Interfaces;

namespace WebRemate.Controllers
{
    public class RemateController : Controller
    {
        private readonly IRemateApiService _remateApiService;

        public RemateController(IRemateApiService remateApiService)
        {
            _remateApiService = remateApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var remates = await _remateApiService.ObtenerTodosLosRemates();
            return View(remates);
        }

        [HttpGet]
        public async Task<IActionResult> SubastasActivas()
        {
            var remates = await _remateApiService.ObtenerSubastasActivas();
            return View(remates);
        }
    }
}
