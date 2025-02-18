using WebRemate.Models;

namespace WebRemate.Interfaces
{
    public interface IProductoApiService
    {
        Task<bool> PublicarProducto(ProductoViewModel producto);
        Task<List<ProductoViewModel>> ObtenerProductosPorRemate(int idRemate);

    }
}
