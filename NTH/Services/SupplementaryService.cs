using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Models.DLPTask;
using NTH.Models.User;
using NTH.Models.Video;
using NTH.Models.Work;

namespace NTH.Services;

public class SupplementaryService
{
#if DEBUG
	private readonly SQLiteContext database;
	public SupplementaryService(SQLiteContext didatabase)
	{
		database = didatabase;
	}

	public void GenerateSupplementaryDefinition()
	{
		database.UserRoleSupplementary.ExecuteDelete();
		database.WorkStatusSupplementary.ExecuteDelete();
		database.WorkTypeSupplementary.ExecuteDelete();
		database.DLPTaskStatusSupplementary.ExecuteDelete();
		database.UserRoleSupplementary.AddRange(UserRoleSupplementary.GetDefinitionList());
		database.WorkStatusSupplementary.AddRange(WorkStatusSupplementary.GetDefinitionList());
		database.WorkTypeSupplementary.AddRange(WorkTypeSupplementary.GetDefinitionList());
		database.DLPTaskStatusSupplementary.AddRange(DLPTaskStatusSupplementary.GetDefinitionList());
		database.SaveChanges();
	}
#endif
}
