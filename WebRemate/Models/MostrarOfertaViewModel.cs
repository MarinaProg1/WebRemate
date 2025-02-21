namespace WebRemate.Models
{
    public class MostrarOfertaViewModel
    {
        public int IdOferta { get; set; }
        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Monto { get; set; }
        public string? Estado { get; set; }
        public string NombreUsuario { get; set; }
    }
}
