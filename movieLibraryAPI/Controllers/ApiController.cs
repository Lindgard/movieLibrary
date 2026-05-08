using Microsoft.AspNetCore.Mvc;
using movieLibrary.Models;

namespace movieLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    /// <summary>
    /// Retrieves a list of TV shows. This is a placeholder implementation that returns a static list of TV show titles. 
    /// In a real application, this would likely query a database or another data source to retrieve the TV show information.
    /// </summary>
    /// <returns></returns>
    [HttpGet("TvShows")]
    public IActionResult GetShows()
    {
        var shows = new List<string> { "Breaking Bad", "The X-Files", "MacGyver" };
        var response = new ApiResponse<List<string>>(true, "Tv shows retrieved successfully", shows);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves a list of movies. This is a placeholder implementation that returns a static list of movie titles. 
    /// In a real application, this would likely query a database or another data source to retrieve the movie information.
    /// </summary>
    /// <returns></returns>
    [HttpGet("Movies")]
    public IActionResult GetMovies()
    {
        var movies = new List<string> { "The Matrix", "Star Wars", "Lord of the Rings" };
        var response = new ApiResponse<List<string>>(true, "Movies retrieved successfully", movies);
        return Ok(response);
    }
}