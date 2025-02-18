using WebRemate.Models;

namespace WebRemate.Interfaces
{
    public interface IRemateApiService
    {
        Task<List<RemateViewModels>> ObtenerSubastasActivas();
        Task<List<RemateViewModels>> ObtenerTodosLosRemates();
    }
}
