using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.Work;

[PrimaryKey(nameof(ID))]
public class WorkTypeSupplementary
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ID { get; set; }
    public required string WorkTypeName { get; set; }
    public static List<WorkTypeSupplementary> GetDefinitionList()
    {
        WorkType[] wtValues = Enum.GetValues<WorkType>();
        return wtValues.Select(
            item => new WorkTypeSupplementary
            {
                ID = (int)item,
                WorkTypeName = item.ToString()
            }).ToList();
    }
}

// [PrimaryKey(nameof(ID))]
// public class EnumSupplementaryTemplate<T> where T : struct, Enum
// {
//     [DatabaseGenerated(DatabaseGeneratedOption.None)]
//     public int ID { get; set; }
//     public required string Name { get; set; }

//     // public static Type enumType = typeof(T);
//     public static List<T> GetDefinitionList()
//     {
//         T[] wtValues = Enum.GetValues<T>();
//         return wtValues.Select(
//             item => new EnumSupplementaryTemplate<T>
//             {
//                 ID = (int)item,
//                 Name = item.ToString()
//             }).ToList();
//     }
// }
