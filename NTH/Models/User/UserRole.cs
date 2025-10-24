using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.User;

public enum UserRole
{
    User = 0,
    Translator = 0b0001,
    Timeliner = 0b0010,
    Manager = 0b0100,

    // This works as intended but hopefully we have the "whole set"
    Administrator = User | Translator | Timeliner | Manager,

    SuperAdministrator = 0x7f_ff_ff_ff,
}

[PrimaryKey(nameof(ID))]
public class UserRoleSupplementary
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ID { get; set; }
    public required string UserRoleName { get; set; }
    public static List<UserRoleSupplementary> GetDefinitionList()
    {
        UserRole[] urValues = Enum.GetValues<UserRole>();
        return urValues.Select(
            item => new UserRoleSupplementary
            {
                ID = (int)item,
                UserRoleName = item.ToString()
            }).ToList();
    }
}
