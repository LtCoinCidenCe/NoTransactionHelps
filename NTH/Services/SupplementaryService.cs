using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Models.User;
using NTH.Models.Video;
using NTH.Models.Work;

namespace NTH.Services;

public class SupplementaryService(SQLiteContext database)
{
#if DEBUG
    public void GenerateSupplementaryDefinition()
    {
        database.UserRoleSupplementary.ExecuteDelete();
        database.WorkStatusSupplementary.ExecuteDelete();
        database.WorkTypeSupplementary.ExecuteDelete();
        database.UserRoleSupplementary.AddRange(UserRoleSupplementary.GetDefinitionList());
        database.WorkStatusSupplementary.AddRange(WorkStatusSupplementary.GetDefinitionList());
        database.WorkTypeSupplementary.AddRange(WorkTypeSupplementary.GetDefinitionList());
        database.SaveChanges();
    }
#endif
}
