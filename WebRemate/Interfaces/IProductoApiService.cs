using WebRemate.Models;

namespace WebRemate.Interfaces
{
    public interface IProductoApiService
    {
        Task<bool> PublicarProducto(CrearProductoViewModel producto);
        Task<List<ProductoViewModel>> ObtenerProductosPorRemate(int idRemate);

    }
}
