using System.ComponentModel.DataAnnotations;

namespace WebRemate.Models
{
    public class CrearProductoViewModel
    {
       
        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio base es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal PrecioBase { get; set; }

        // Propiedad para recibir la imagen desde el formulario
        [Required(ErrorMessage = "Debe seleccionar una imagen")]
        public IFormFile Imagen { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un remate")]
        public int IdRemate { get; set; }

        // Este ID debería asignarse automáticamente desde el usuario autenticado
        public int IdUsuario { get; set; }
    }
}
