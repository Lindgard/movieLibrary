namespace movieLibrary.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    /// <summary>
    /// Initializes a new instance of the ApiResponse class with the specified success status, message, and optional data.
    /// </summary>
    /// <param name="success">Indicates whether the API request was successful.</param>
    /// <param name="message">A message providing additional information about the API response.</param>
    /// <param name="data">The data returned by the API, if any.</param>
    public ApiResponse(bool success, string message, T? data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}