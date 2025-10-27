using System.ComponentModel.DataAnnotations;

namespace NTH.DTO.User;

public class NewUserDTO
{
    [Length(2, 30)]
    public required string Username { get; set; }
    [Length(2, 30)]
    public required string Displayname { get; set; }
    [Length(5, 90)]
    public required string Password { get; set; }
}
