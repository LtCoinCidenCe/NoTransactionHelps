using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NTH.Models.User;

public class UserBigText
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long UserID { get; set; }


    [JsonIgnore]
    [Column(nameof(UserID))]
    public UserID? User { get; set; }
    public string Content { get; set; } = "";
    public int times { get; set; } = 1;
}
