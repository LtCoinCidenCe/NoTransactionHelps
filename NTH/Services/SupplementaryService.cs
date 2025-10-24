using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Models.User;

namespace NTH.Services;

public class SupplementaryService(PostgresContext database)
{
    public void GenerateSupplementaryDefinition()
    {
        database.UserRoleSupplementary.ExecuteDelete();
        database.UserRoleSupplementary.AddRange(UserRoleSupplementary.GetDefinitionList());
        database.SaveChanges();
    }
}
