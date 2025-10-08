#if DEBUG
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NTH.DBContext;
using NTH.Models;

namespace NTH.Controllers;

[ApiController]
[Route("Debug")]
public class DebugController(ILogger<DebugController> logger, PostgresContext database) : ControllerBase
{
    [HttpDelete]
    public void InitializeDatabase()
    {
        logger.Log(LogLevel.Warning, "Database Reinitialized");
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();

        string firstDisplayname = "The First Emperor", defaultPassword = "someDefault", defaultSalt = "sirow";
        byte[] hash = Encoding.UTF8.GetBytes(defaultSalt + defaultPassword);
        for (int i = 0; i < 5; i++)
        {
            hash = SHA256.HashData(hash);
        }
        var firstUser = new UserID()
        {
            Username = "FirstUser",
            Displayname = firstDisplayname,
            Password = hash,
            PassSalt = defaultSalt
        };
        database.Users.Add(firstUser);
        database.SaveChanges();
        logger.Log(LogLevel.Warning, JsonSerializer.Serialize(firstUser));

        var historyItem = new DisplaynameHistory()
        {
            Displayname = firstDisplayname,
            User = firstUser,
            CreationDate = firstUser.CreationDate
        };
        database.DisplaynameHistories.Add(historyItem);
        database.SaveChanges();
    }

    [HttpGet]
    [Route("[action]")]
    public void donothing()
    {
    }
}
#endif
