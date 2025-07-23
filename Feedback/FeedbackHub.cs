using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace charac.Hubs
{
    public class FeedbackHub : Hub
    {
        public async Task SendMessage(string user, string message, int id, int likes)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, id, likes);
        }
    }
}
