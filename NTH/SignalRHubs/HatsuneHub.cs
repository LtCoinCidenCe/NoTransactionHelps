using Microsoft.AspNetCore.SignalR;

namespace NTH.SignalRHubs;

public class HatsuneHub : Hub
{
	public async Task SendMessagetoChat(string user, string message)
	{
		await Clients.All.SendAsync("Chatting", new ChatMessage() { User = user, Message = message });
	}
}

public class ChatMessage
{
	public required string User { get; set; }
	public required string Message { get; set; }
}
