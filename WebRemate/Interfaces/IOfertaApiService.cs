using WebRemate.Models;

namespace WebRemate.Interfaces
{
    public interface IOfertaApiService
    {
        
        Task<(bool Exito, string Mensaje)> RealizarOferta(OfertaViewModel model);
        Task<List<MostrarOfertaViewModel>> ObtenerOfertas(int idProducto);
        Task<(bool Exito, string Mensaje, FacturaViewModel? Factura)> SeleccionarOfertaGanadora(int idProducto);
    }
}
