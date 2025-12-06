using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.Work;

[PrimaryKey(nameof(ID))]
public class WorkStatusSupplementary
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ID { get; set; }
    public required string WorkStatusName { get; set; }
    public static List<WorkStatusSupplementary> GetDefinitionList()
    {
        WorkStatus[] wsValues = Enum.GetValues<WorkStatus>();
        return wsValues.Select(
            item => new WorkStatusSupplementary
            {
                ID = (int)item,
                WorkStatusName = item.ToString()
            }).ToList();
    }
}
