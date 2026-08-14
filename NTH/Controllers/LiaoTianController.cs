using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NTH.Controllers.Filters;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.LiaoTian;
using NTH.SignalRHubs;
using System.ComponentModel.DataAnnotations;

namespace NTH.Controllers;

/// <summary>
/// 聊天记录按ID存预计顺序且连续无删除
/// </summary>
/// <param name="database"></param>
/// <param name="liaotianHub"></param>
/// <param name="requestingUser"></param>
[Authorize, ApiController, Route("api/LiaoTian")]
public class LiaoTianController([FromServices] SQLiteContext database,
	[FromServices] IHubContext<LiaoTianHub> liaotianHub,
	[FromServices] RequestingUser requestingUser) : ControllerBase
{
	[HttpPost, Route("ShuoHua"), NewMessageFilter]
	public async Task<IActionResult> NewMessage(NewMessageDTO newMessage)
	{
		var newDBmessage = new Message { UserID = requestingUser.UserID, Words = newMessage.Words };
		await database.LiaoTianJiLu.AddAsync(newDBmessage);
		await database.SaveChangesAsync();

		await liaotianHub.Clients.All.SendAsync("newest", newDBmessage);
		return Ok("OK");
	}

	[HttpGet, Route("History")]
	public IQueryable<Message> GetChatHistory([FromQuery, Required] long lastReceivedChatID)
	{
		long? max = database.LiaoTianJiLu.Max(x => (long?)x.ID); // haha just convert to long? to avoid exception when table is empty
		if (lastReceivedChatID + 70 < max)
			return database.LiaoTianJiLu.Where(x => x.ID > lastReceivedChatID);
		// history too long return newest instead
		return database.LiaoTianJiLu.OrderByDescending(x => x.ID).Take(70);
	}

	/// <summary>
	/// 往前指定读70条
	/// </summary>
	/// <param name="oldestMsg"></param>
	/// <returns></returns>
	[HttpGet, Route("Take70")]
	public IQueryable<Message> Take70Message([FromQuery, Required] long oldestMsg)
	{
		var leftEnd = oldestMsg - 70;
		return database.LiaoTianJiLu.Where(x => x.ID >= leftEnd && x.ID < oldestMsg);
	}
}

public struct NewMessageDTO
{
	[MaxLength(Message.MaxLength)]
	public required string Words { get; set; }
}
