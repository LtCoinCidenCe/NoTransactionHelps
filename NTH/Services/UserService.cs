using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.User;
using NTH.Models.User;
using NTH.Utilities;

namespace NTH.Services;

public class UserService(PostgresContext database)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID">can be number or username</param>
    /// <returns></returns>
    public UserID? GetUserByID(string ID)
    {
        if (long.TryParse(ID, out var numID))
        {
            return database.Users.AsNoTracking().FirstOrDefault(x => x.ID == numID && !x.IsDeleted);
        }
        else
        {
            return database.Users.AsNoTracking().FirstOrDefault(x => x.Username == ID && !x.IsDeleted);
        }
    }

    public UserLoginResponse? Login(UserLoginDTO userLoginDTO)
    {
        return database.Users.AsNoTracking().Where(x => x.Username == userLoginDTO.Username && !x.IsDeleted).Select(x => new UserLoginResponse { PassSalt = x.PassSalt, Password = x.Password }).FirstOrDefault();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="newDisplayName"></param>
    /// <param name="dateTime"></param>
    /// <returns><b>null</b> if the user is not found. Else the new Display name history object</returns>
    public DisplaynameHistory? SetDisplayName(long ID, string newDisplayName, DateTimeOffset? dateTime)
    {
        UserID? user = database.Users.FirstOrDefault(x => x.ID == ID && !x.IsDeleted);
        if (user is null)
            return null;
        DateTimeOffset transactionTime = dateTime.GetValueOrDefault(DateTimeOffset.UtcNow);

        DisplaynameHistory newHistory = new() { Displayname = newDisplayName, CreationDate = transactionTime, User = user };
        database.UserDisplaynameHistories.Add(newHistory);

        user.Displayname = newDisplayName;
        user.DisplaynameChangeDate = transactionTime;
        database.SaveChanges();
        return newHistory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="newTitleWords"></param>
    /// <returns>updated rows</returns>
    public int SetTitleWords(long ID, string newTitleWords)
    {
        var now = DateTimeOffset.UtcNow;
        int updated = database.Users
            .Where(user => user.ID == ID && !user.IsDeleted)
            .ExecuteUpdate(setter => setter
                .SetProperty(user => user.TitleWords, newTitleWords)
                .SetProperty(user => user.TitleWordsChangeDate, now));
        return updated;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="password"></param>
    /// <returns>updated rows</returns>
    public int SetPassword(long ID, string password)
    {
        string? salt = null;
        var hash = PasswordHasher.GetHashedPassword(password, ref salt);
        if (salt is null)
            throw new PasswordHasherException("salt is not received");

        var now = DateTimeOffset.UtcNow;
        int updated = database.Users
            .Where(user => user.ID == ID)
            .ExecuteUpdate(setter => setter
                .SetProperty(user => user.PassSalt, salt)
                .SetProperty(user => user.Password, hash)
                .SetProperty(user => user.PasswordChangeDate, now));
        return updated;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="newRole"></param>
    /// <returns><b>null</b> if the user is not found. Else the new User Role History object</returns>
    public UserRoleHistory? SetUserRole(long ID, UserRoleDTO newRole)
    {
        UserID? user = database.Users.FirstOrDefault(user => user.ID == ID && !user.IsDeleted);
        if (user is null)
            return null;
        var dateTime = DateTimeOffset.UtcNow;

        UserRoleHistory newRow = new() { UserID = ID, UserRole = newRole, CreationDate = dateTime };
        database.UserRoleHistories.Add(newRow);

        user.UserRole = newRole;
        user.UserRoleChangeDate = dateTime;

        database.SaveChanges();
        return newRow;
    }

    /// <summary>
    /// This uses mutex to check username.
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns><b>null</b> if the username is already taken. Else the new created UserID</returns>
    /// <exception cref="Exception"></exception>
    public UserID? CreateNewUser(NewUserDTO newUser)
    {
        string? salt = null;
        byte[] hashed = PasswordHasher.GetHashedPassword(newUser.Password, ref salt);
        if (salt is null)
            throw new PasswordHasherException("salt is not received");

        UserID userID = new()
        {
            Username = newUser.Username,
            Displayname = newUser.Displayname,
            Password = hashed,
            PassSalt = salt
        };
        try
        {
            usernameTraffic.WaitOne();

            var existingUsername = database.Users
                .Where(x => x.Username == newUser.Username && !x.IsDeleted)
                .Select(x => new { x.ID, x.Username })
                .FirstOrDefault();
            if (existingUsername is not null)
                return null;
            database.Users.Add(userID);
            database.SaveChanges();
        }
        finally
        {
            usernameTraffic.ReleaseMutex();
        }

        userID.DisplaynameHistory.Add(new DisplaynameHistory
        {
            UserID = userID.ID,
            Displayname = userID.Displayname,
            CreationDate = userID.CreationDate,
        });
        database.SaveChanges();
        return userID;
    }
    public static Mutex usernameTraffic = new();
}



public class PasswordHasherException : Exception
{
    public PasswordHasherException() : base()
    {
    }
    public PasswordHasherException(string? message) : base(message)
    {
    }
    public PasswordHasherException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}

public class UserLoginResponse
{
    public required string PassSalt;
    public required byte[] Password;
}
