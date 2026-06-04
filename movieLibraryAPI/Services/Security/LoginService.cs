namespace movieLibraryAPI.Services.Security;

public class LoginService
{
    /// <summary>
    /// Authenticates a user based on the provided username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>True if the user is authenticated; otherwise, false.</returns>
    public bool Login(string username, string password)
    {
        //* Placeholder for actual authentication logic, such as checking against a database
        if (username == "admin" && password == "password") return true;
        return false;
    }
}