using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Auth
{
    public interface ILoginService
    {
        Task Login(string token);

        Task Logout();
    }
}
