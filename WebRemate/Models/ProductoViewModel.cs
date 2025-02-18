using System.ComponentModel.DataAnnotations;

namespace WebRemate.Models
{
    public class ProductoViewModel
    {
        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal PrecioBase { get; set; }

        [Required]
        public IFormFile Imagen { get; set; }

        [Required]
        public int IdRemate { get; set; }

        [Required]
        public int IdUsuario { get; set; } // Este ID debe venir del usuario autenticado
    }
}
