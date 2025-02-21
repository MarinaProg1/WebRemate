
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebRemate.Interfaces;
using WebRemate.Models;
using System.Threading.Tasks;

namespace WebRemate.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly IUsuarioApiService _usuarioApiService;

        public AutenticacionController(IUsuarioApiService usuarioApiService)
        {
            _usuarioApiService = usuarioApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tokenResponse = await _usuarioApiService.Login(model);
                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.token))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, model.Email),
                        new Claim("Token", tokenResponse.token)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                    return RedirectToAction("Index", "Remate");
                }

                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _usuarioApiService.Logout();
            return RedirectToAction("Index", "Remate");
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(RegistroUsuarioViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                bool success = await _usuarioApiService.Registrarse(modelo);
                if (success)
                {
                    TempData["Exito"] = "¡Registro exitoso! Ahora puedes <a href='/Autenticacion/Login'>iniciar sesión</a>.";
                    return View(modelo); // Permanece en la misma vista para mostrar el mensaje
                }

                TempData["Error"] = "Hubo un problema al registrar el usuario.";
            }

            return View(modelo);
        }


    }
}
