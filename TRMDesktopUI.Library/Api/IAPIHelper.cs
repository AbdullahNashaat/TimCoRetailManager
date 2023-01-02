using System.Threading.Tasks;


namespace TRMDesktopUI.Library.Models
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
    }
}