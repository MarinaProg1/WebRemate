namespace WebRemate.Models
{
    public class FacturaViewModel
    {
        public int IdFactura { get; set; }
        public int IdOferta { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
    }
}
