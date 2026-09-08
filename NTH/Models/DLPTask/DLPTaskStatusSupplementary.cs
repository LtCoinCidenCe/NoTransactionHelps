using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NTH.Models.DLPTask;

[PrimaryKey(nameof(ID))]
public class DLPTaskStatusSupplementary
{
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }
	public required string DLPTaskStatusName { get; set; }
	public static List<DLPTaskStatusSupplementary> GetDefinitionList()
	{
		DLPTaskStatus[] dlpTaskStatusValues = Enum.GetValues<DLPTaskStatus>();
		return dlpTaskStatusValues.Select(
			item => new DLPTaskStatusSupplementary
			{
				ID = (int)item,
				DLPTaskStatusName = item.ToString()
			}).ToList();
	}
}
