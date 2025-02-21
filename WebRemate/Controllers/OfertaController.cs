﻿using Microsoft.AspNetCore.Mvc;
using WebRemate.Models;
using WebRemate.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using WebRemate.Service;

namespace WebRemate.Controllers
{
    public class OfertaController : Controller
    {
        private readonly IOfertaApiService _ofertaApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OfertaController(IOfertaApiService ofertaApiService, IHttpContextAccessor httpContextAccessor)
        {
            _ofertaApiService = ofertaApiService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Método GET para mostrar el formulario de oferta
        [HttpGet]
        public IActionResult Oferta(int idProducto)
        {
            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Autenticacion");
            }

            // Decodificar el token para obtener el ID del usuario
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var idUsuarioClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                return Challenge(); // Redirigir si no se encuentra el claim o el ID no es válido
            }



            // Crear el modelo con el IdProducto
            var model = new OfertaViewModel
            {
                IdProducto = idProducto,
                IdUsuario = idUsuario
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Oferta(OfertaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Se deben completar todos los campos requeridos.");
                return View(model); // Si el modelo no es válido, se retorna la vista inmediatamente
            }

            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Autenticacion");
            }

            // Obtener el IdUsuario desde el token JWT
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var idUsuarioClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                return RedirectToAction("Login", "Autenticacion");
            }

            // Asignar el IdUsuario al modelo
            model.IdUsuario = idUsuario;

            // Llamar al servicio para realizar la oferta con el modelo completo
            var (exito, mensaje) = await _ofertaApiService.RealizarOferta(model);

            if (exito)
            {
                TempData["Exito"] = mensaje; // Mensaje de éxito
                return View(model); // Permanece en la misma vista
            }

            // Si hay un error, mostrar el mensaje
            TempData["Error"] = mensaje; // Mensaje de error
            return View(model);
        }



        [HttpGet]
            public async Task<IActionResult> OfertaPorProducto(int idProducto)
            {
                var ofertas = await _ofertaApiService.ObtenerOfertas(idProducto);
                return View(ofertas);

            }

        [HttpPost]
        public async Task<IActionResult> SeleccionarGanadora(int idProducto)
        {
            var (Exito, Mensaje, Factura) = await _ofertaApiService.SeleccionarOfertaGanadora(idProducto);

            if (Exito)
            {
                ViewBag.Mensaje = Mensaje;
                return View("FacturaGenerada", Factura);
            }

            ViewBag.Error = Mensaje;
            return View("Error");
        }

    }
    }
