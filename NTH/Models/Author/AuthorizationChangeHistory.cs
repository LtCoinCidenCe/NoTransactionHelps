using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NTH.Models.Author;

public class AuthorizationChangeHistory
{
    public long ID { get; set; }
    [Column(name: "AuthorID")]
    public long AuthorID { get; set; }
    [Column(name: "AuthorID")]
    [JsonIgnore]
    public AuthorID? Author { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;

    public bool AuthorizedPerVideo { get; set; } = false;
    public bool AllVideoAuthorized { get; set; } = false;
}
