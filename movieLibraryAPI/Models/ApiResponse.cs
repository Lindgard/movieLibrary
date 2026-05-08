namespace movieLibrary.Models;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}