using NTH.DBContext;
using NTH.Models.Author;

namespace NTH.Services;

public class AuthorService(SQLiteContext database)
{
    public AuthorizationChangeHistory? SetAuthorization(long ID, bool AuthorizedPerVideo, bool AllVideoAuthorized, DateTimeOffset? dateTime)
    {
        bool hasAuthor = database.Authors.Any(x => x.ID == ID);
        if (!hasAuthor)
            return null;
        DateTimeOffset transactionTime = dateTime.GetValueOrDefault(DateTimeOffset.UtcNow);
        AuthorizationChangeHistory newHistory = new()
        {
            AuthorID = ID,
            AuthorizedPerVideo = AuthorizedPerVideo,
            AllVideoAuthorized = AllVideoAuthorized,
            CreationDate = transactionTime
        };
        database.AuthorizationChangeHistories.Add(newHistory);
        database.SaveChanges();
        return newHistory;
    }
}
