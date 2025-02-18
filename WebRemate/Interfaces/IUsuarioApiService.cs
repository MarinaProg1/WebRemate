using WebRemate.Models;
using System.Threading.Tasks;

namespace WebRemate.Interfaces
{
    public interface IUsuarioApiService
    {
        Task<TokenResponse?> Login(LoginViewModel model);
        Task<bool> Registrarse(RegistroUsuarioViewModel modelo);
        void Logout();
        string? ObtenerToken();
    }
}
