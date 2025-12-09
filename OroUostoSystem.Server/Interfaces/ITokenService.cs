using OroUostoSystem.Server.Models;

namespace OroUostoSystem.Server.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
