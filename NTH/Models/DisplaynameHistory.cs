using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models;

[Index(nameof(CreationDate))]
public class DisplaynameHistory
{
    public long ID { get; set; }

    [Column(name: "UserID")]
    public long UserID { get; set; }
    [Column(name: "UserID")]
    [JsonIgnore]
    public UserID? User { get; set; }

    [MaxLength(30)]
    public required string Displayname { get; set; }

    public required DateTimeOffset CreationDate { get; set; }

    public bool IsDeleted { get; set; } = false;
}
