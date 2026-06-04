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

    /// <summary>
    /// Hashes the provided password using a simple encoding method. 
    /// In a real application, you should use a secure hashing algorithm like bcrypt or Argon2.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}