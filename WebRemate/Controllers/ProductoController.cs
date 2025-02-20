
//using Microsoft.AspNetCore.Mvc;
//using WebRemate.Interfaces;
//using WebRemate.Models;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;
//[Authorize]
//public class ProductoController : Controller
//{
//    private readonly IProductoApiService _productoApiService;

//    public ProductoController(IProductoApiService productoApiService)
//    {
//        _productoApiService = productoApiService;
//    }

//    [HttpGet]
//    public async Task<IActionResult> ProductosPorRemate(int idRemate)
//    {
//        if (!User.Identity.IsAuthenticated)
//        {
//            return RedirectToAction("Login", "Autenticacion");
//        }
//        var productos = await _productoApiService.ObtenerProductosPorRemate(idRemate);

//        if (productos == null)
//        {
//            // Puedes pasar un mensaje a la vista o redirigir a otra página.
//            return View("Error"); // O una vista personalizada que indique que no hay productos.
//        }


//        return View(productos);
//    }


//    [HttpGet]
//    public IActionResult Publicar(int idRemate)
//    {
//        // Obtener el token desde la cookie
//        var token = Request.Cookies["jwt_token"];

//        if (string.IsNullOrEmpty(token))
//        {
//            return RedirectToAction("Login", "Autenticacion"); // Redirigir al login si no hay token
//        }

//        // Decodificar el token y extraer el ID del usuario
//        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
//        var jwtToken = handler.ReadJwtToken(token);
//        var idUsuarioClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

//        if (idUsuarioClaim == null)
//        {
//            return RedirectToAction("Login", "Autenticacion"); // Redirigir si no se encuentra el claim
//        }

//        // Convertir el ID del usuario a entero
//        int idUsuario = int.Parse(idUsuarioClaim.Value);

//        // Crear el modelo con los valores preasignados
//        var model = new ProductoViewModel
//        {
//            IdRemate = idRemate,
//            IdUsuario = idUsuario
//        };

//        return View(model); // Pasamos el modelo a la vista
//    }

//    [HttpPost]
//    public async Task<IActionResult> Publicar(ProductoViewModel model)
//    {
//        if (!ModelState.IsValid)
//        {
//            return View(model); // Si el modelo no es válido, regresar la vista con los errores
//        }

//        // Verificamos si se ha subido una imagen
//        if (model.Imagen != null && model.Imagen.Length > 0)
//        {
//            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
//            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Imagen.FileName;
//            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await model.Imagen.CopyToAsync(stream);
//            }


//        }
//        else
//        {
//            ModelState.AddModelError(string.Empty, "Debe seleccionar una imagen.");
//            return View(model);
//        }

//        // Llamamos al servicio para publicar el producto
//        bool publicado = await _productoApiService.PublicarProducto(model);

//        if (!publicado)
//        {
//            ModelState.AddModelError(string.Empty, "Error al publicar el producto.");
//            return View(model);
//        }

//        return RedirectToAction("ProductosPorRemate", new { idRemate = model.IdRemate });
//    }



//}


using Microsoft.AspNetCore.Mvc;
using WebRemate.Interfaces;
using WebRemate.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;
using System.IO;

[Authorize]
public class ProductoController : Controller
{
    private readonly IProductoApiService _productoApiService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ProductoController(IProductoApiService productoApiService, IHttpContextAccessor httpContextAccessor)
    {
        _productoApiService = productoApiService;
    }

    [HttpGet]
    public async Task<IActionResult> ProductosPorRemate(int idRemate)
    {
        // Si el usuario no está autenticado, forzar login
        if (!User.Identity.IsAuthenticated || User.Identity == null)
        {
            return Challenge(); // Redirige automáticamente al login
        }

        var productos = await _productoApiService.ObtenerProductosPorRemate(idRemate);

        if (productos == null || !productos.Any())
        {
            ViewBag.ErrorMessage = "No hay productos en este remate.";
            return View("Error"); // Vista personalizada para errores
        }

        return View(productos);
    }

    [HttpGet]
    public IActionResult Publicar(int idRemate)
    {
        // Obtener el token desde la cookie
        var token = Request.Cookies["jwt_token"];

        if (string.IsNullOrEmpty(token))
        {
            return Challenge(); // Redirigir al login si no hay token
        }

        try
        {
            // Decodificar el token y extraer el ID del usuario
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var idUsuarioClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idUsuarioClaim == null)
            {
                return Challenge(); // Redirigir si no se encuentra el claim
            }

            // Convertir el ID del usuario a entero
            if (!int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                return Challenge(); // Redirigir si el ID no es válido
            }

            // Crear el modelo con los valores preasignados
            var model = new ProductoViewModel
            {
                IdRemate = idRemate,
                IdUsuario = idUsuario
            };

            return View(model); // Pasamos el modelo a la vista
        }
        catch (Exception)
        {
            return Challenge(); // Si hay un error, redirigir al login
        }
    }
    [HttpPost]
    public async Task<IActionResult> Publicar(ProductoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Obtener el token desde la cookie
        var token = Request.Cookies["jwt_token"];

        if (string.IsNullOrEmpty(token))
        {
            return Challenge(); // Redirigir al login si no hay token
        }

        // Decodificar el token para obtener el ID del usuario
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var idUsuarioClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
        {
            return Challenge(); // Redirigir si no se encuentra el claim o el ID no es válido
        }

        model.IdUsuario = idUsuario;

        bool publicado = await _productoApiService.PublicarProducto(model);

        if (!publicado)
        {
            ModelState.AddModelError(string.Empty, "Error al publicar el producto.");
            return View(model);
        }

        return RedirectToAction("ProductosPorRemate", new { idRemate = model.IdRemate });
    }


}
