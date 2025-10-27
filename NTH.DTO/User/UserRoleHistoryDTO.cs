namespace NTH.DTO.User;

public class UserRoleHistoryDTO
{
    // public long ID { get; set; }
    public long UserID { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    public UserRoleDTO UserRole { get; set; }
}
