using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.Video;
using NTH.Utilities;

namespace NTH.Controllers;

[Authorize]
[ApiController]
[Route("api/Work")]
public class WorkController(SQLiteContext database, RequestingUser requestingUser) : ControllerBase
{
	[HttpPatch]
	[Route("{vID}/Title")]
	public IActionResult SetTitleTranslation(long vID, [FromBody, MaxLength(VideoID.MAX_TRANSEDTITLE)] string translatedTitle)
	{
		var timeNow = DateTimeOffset.UtcNow;
		int updation = database.Videos.Where(x => x.ID == vID).ExecuteUpdate(
			setter => setter.SetProperty(v => v.WTitleTranslation, translatedTitle)
				.SetProperty(v => v.WTitleChangeUser, requestingUser.UserID)
				.SetProperty(v => v.WTitleChangeDate, timeNow));
		if (updation == 0)
			return NotFound();
		if (updation == 1)
			return Ok("OK");
		else
			throw new NTHException($"{nameof(SetTitleTranslation)} updated unknown impossible records");
	}

	[HttpPatch]
	[Route("{vID}/Intro")]
	public IActionResult SetIntroductionTranslation(long vID, [FromBody, MaxLength(VideoID.MAX_TRANSEDINTRO)] string translatedIntroduction)
	{
		var timeNow = DateTimeOffset.UtcNow;
		int updation = database.Videos.Where(x => x.ID == vID).ExecuteUpdate(
			setter => setter.SetProperty(v => v.WIntroTranslation, translatedIntroduction)
				.SetProperty(v => v.WIntroChangeUser, requestingUser.UserID)
				.SetProperty(v => v.WIntroChangeDate, timeNow)
				.SetProperty(v => v.StatusTranslation,
				ov => ov.StatusTranslation >= WorkStatus.InProgress ? ov.StatusTranslation : WorkStatus.InProgress));
		if (updation == 0)
			return NotFound();
		if (updation == 1)
			return Ok("OK");
		else
			throw new NTHException($"{nameof(SetIntroductionTranslation)} updated unknown impossible records");
	}
}
