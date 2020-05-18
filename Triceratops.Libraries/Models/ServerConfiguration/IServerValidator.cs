using System.Threading.Tasks;

namespace Triceratops.Libraries.Models.ServerConfiguration
{
    public interface IServerValidator
    {
        Task ValidateServerAsync(Server server);
    }
}
