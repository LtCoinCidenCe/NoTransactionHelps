namespace NTH.DTO.User;

public class DisplaynameHistoryDTO
{
    // public long ID { get; set; }
    public long UserID { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    public required string Displayname { get; set; }
}
