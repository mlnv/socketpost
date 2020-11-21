using System.Threading.Tasks;

namespace Socketpost.Services.WebSocket
{
    public interface IService
    {
        public Task Connect(string uri);
        public Task<bool> Disconnect();
    }
}
