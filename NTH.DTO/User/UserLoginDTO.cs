using System.ComponentModel.DataAnnotations;

namespace NTH.DTO.User;

public class UserLoginDTO
{
    [Length(2, 30)]
    public required string Username { get; set; }
    [Length(5, 90)]
    public required string Password { get; set; }
}
