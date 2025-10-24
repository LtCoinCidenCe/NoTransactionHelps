using System.ComponentModel.DataAnnotations;

namespace NTH.DTO.User;

public class NewUser
{
    [Length(2, 30)]
    public required string Username { get; set; }
    [Length(2, 30)]
    public required string Displayname { get; set; }
    [MinLength(5)]
    public required string Password { get; set; }
}
