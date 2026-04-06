using Microsoft.AspNetCore.SignalR;

namespace NTH.SignalRHubs;

public class HatsuneHub : Hub
{
	public async Task SendMessagetoChat(string user, string message)
	{
		var userClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "aud");
		var userReplied = userClaim?.Value.Substring(2);
		await Clients.All.SendAsync("Chatting", new ChatMessage() { User = userReplied ?? "", Message = message });
	}
}

public class ChatMessage
{
	public required string User { get; set; }
	public required string Message { get; set; }
}
