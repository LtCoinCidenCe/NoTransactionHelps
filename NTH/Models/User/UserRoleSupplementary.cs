using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.User;

[PrimaryKey(nameof(ID))]
public class UserRoleSupplementary
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ID { get; set; }
    public required string UserRoleName { get; set; }
    public static List<UserRoleSupplementary> GetDefinitionList()
    {
        UserRoleDTO[] urValues = Enum.GetValues<UserRoleDTO>();
        return urValues.Select(
            item => new UserRoleSupplementary
            {
                ID = (int)item,
                UserRoleName = item.ToString()
            }).ToList();
    }
}
