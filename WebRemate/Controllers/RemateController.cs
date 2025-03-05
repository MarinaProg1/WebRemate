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

            // Verifica si la lista está vacía
            if (remates == null || !remates.Any())
            {
                ViewBag.Mensaje = "No hay remates disponibles en este momento.";
            }

            return View(remates);
        }


        [HttpGet]
        public async Task<IActionResult> SubastasActivas()
        {
            var remates = await _remateApiService.ObtenerSubastasActivas();
            return View(remates);
        }

        [HttpGet]
        public async Task<IActionResult> CalcularOfertaGanadora(int idProducto)
        {
            try
            {
                var ganador = await _remateApiService.CalcularOfertaGanadoraPorProducto(idProducto);

                if (ganador == null)
                {
                    ViewBag.Mensaje = "No hay oferta ganadora para este producto.";
                    return View("SinGanador");
                }

                return View("Ganador", ganador);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> OfertasGanadoras()
        {
            var ofertas = await _remateApiService.CalcularOfertasGanadoras();
            return View(ofertas);
        }
    }
}

