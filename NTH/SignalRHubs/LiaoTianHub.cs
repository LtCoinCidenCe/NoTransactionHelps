using Microsoft.AspNetCore.SignalR;
using NTH.Models.LiaoTian;

namespace NTH.SignalRHubs;

#pragma warning disable CS9113 // 参数未读。
public class LiaoTianHub(ILogger<LiaoTianHub> logger) : Hub<List<Message>>
#pragma warning restore CS9113 // 参数未读。
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
