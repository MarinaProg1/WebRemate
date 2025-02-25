
using System.ComponentModel.DataAnnotations;

namespace WebRemate.Models
{
    public class ProductoViewModel
    {

        public int IdProducto { get; set; }
        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio base es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal PrecioBase { get; set; }

       
        public string Imagenes { get; set; }
        public string? Estado { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un remate")]
        public int IdRemate { get; set; }
        public int IdUsuario { get; set; }
    }
}
