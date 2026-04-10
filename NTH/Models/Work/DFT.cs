using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.Work;

[PrimaryKey(nameof(CreationDate), nameof(ID))]
public class DFT
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTimeOffset CreationDate { get; set; }
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long ID { get; set; }
	public required string Content { get; set; }
}
