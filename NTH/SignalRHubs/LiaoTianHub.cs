using Microsoft.AspNetCore.SignalR;
using NTH.Models.LiaoTian;

namespace NTH.SignalRHubs;

public class LiaoTianHub(ILogger<LiaoTianHub> logger) : Hub<List<Message>>
{
	public override async Task OnConnectedAsync()
	{
		// Unavailable
		//var caller = Clients.Caller;
		//var omni = Clients.All;
		//var others = Clients.Others;

		return;
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
	}
}
