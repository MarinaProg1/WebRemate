using WebRemate.Models;

namespace WebRemate.Interfaces
{
    public interface IRemateApiService
    {
        Task<List<RemateViewModels>> ObtenerSubastasActivas();
        Task<List<RemateViewModels>> ObtenerTodosLosRemates();
        Task<RemateViewModels> ObtenerRematePorId(int idRemate);
        Task<GanadorViewModel> CalcularOfertaGanadoraPorProducto(int idProducto);
        Task<List<OfertaGanadoraViewModel>> CalcularOfertasGanadoras();


 }
}
