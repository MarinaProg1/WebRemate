//using System.ComponentModel.DataAnnotations;

//namespace WebRemate.Models
//{
//    public class ProductoViewModel
//    {
//        [Required]
//        public string Titulo { get; set; }

//        [Required]
//        public string Descripcion { get; set; }

//        [Required]
//        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
//        public decimal PrecioBase { get; set; }

//        // 🛠 Agregar la propiedad para almacenar la URL de la imagen
//        public string ImagenUrl { get; set; }

//        // 📷 Propiedad para recibir la imagen en el formulario
//        public IFormFile Imagen { get; set; }

//        [Required]
//        public int IdRemate { get; set; }

//        [Required]
//        public int IdUsuario { get; set; } // Este ID debe venir del usuario autenticado
//    }
//}
using System.ComponentModel.DataAnnotations;

namespace WebRemate.Models
{
    public class ProductoViewModel
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
