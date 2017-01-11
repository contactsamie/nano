using Microsoft.AspNet.SignalR;

namespace Nano.WebUIHost
{
    public class NanoServiceHub : Hub
    {
        public void PerformOperation(string operation, string id, int quantity, int retryCount)
        {
        }
    }
}