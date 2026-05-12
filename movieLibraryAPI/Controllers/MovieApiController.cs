using Microsoft.AspNetCore.Mvc;
using movieLibrary.Models.Response;
using movieLibrary.Services;
using movieLibrary.Models.DTOs;
using movieLibrary.Mappings;
using movieLibrary.Models.Domain;

namespace movieLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieApiController : ControllerBase
{
    private readonly MovieService? _movieService;

    public MovieApiController(MovieService? movieService)
    {
        _movieService = movieService;
    }

    /// <summary>
    /// Retrieves a list of movies. This is a placeholder implementation that returns a static list of movie titles. 
    /// In a real application, this would likely query a database or another data source to retrieve the movie information.
    /// </summary>
    /// <returns>A list of movies.</returns>
    [HttpGet("Movies")]
    public async Task<IActionResult> GetMovies()
    {
        if (_movieService == null)
        {
            return StatusCode(500, new ApiResponse<List<MovieDTO>>
            {
                StatusCode = 500,
                Message = "Movie service is not available",
                Data = null
            });
        }

        var movies = await _movieService.GetAllMoviesAsync();
        var movieDTOs = movies.Select(m => m.ToDto()).ToList();
        var response = new ApiResponse<List<MovieDTO>>
        {
            StatusCode = 200,
            Message = "Movies retrieved successfully",
            Data = movieDTOs
        };
        return Ok(response);
    }

    /// <summary>
    /// Adds a new movie to the collection. This is a placeholder implementation that simulates adding a movie. 
    /// In a real application, this would likely accept a movie object in the request body and save it to a database or another data source.
    /// </summary>
    /// <param name="newMovie">The movie object containing the details of the new movie.</param>
    /// <returns>The created movie object.</returns>
    [HttpPost("AddMovie")]
    public async Task<IActionResult> AddMovie(CreateMovieDTO newMovie)
    {
        if (_movieService == null)
        {
            return StatusCode(500, new ApiResponse<CreateMovieDTO>
            {
                StatusCode = 500,
                Message = "Movie service is not available",
                Data = null
            });
        }
        var domainModel = newMovie.ToDomain();
        var createdMovie = await _movieService.AddMovieAsync(domainModel);
        var responseDTO = new ApiResponse<MovieDTO>
        {
            StatusCode = 201,
            Message = "Movie added successfully",
            Data = createdMovie.ToDto()
        };
        return CreatedAtAction(nameof(GetMovies), new { title = createdMovie.Title }, responseDTO);
    }

    /// <summary>
    /// Removes a movie from the collection. This is a placeholder implementation that simulates removing a movie. 
    /// In a real application, this would likely accept an identifier (such as a movie ID or title) in the request 
    /// and remove the corresponding movie from a database or another data source.
    /// </summary>
    /// <param name="title">The title of the movie to remove.</param>
    /// <returns>The removed movie object if found; otherwise, null.</returns>
    [HttpDelete("RemoveMovie")]
    public async Task<IActionResult> RemoveMovie(string title)
    {
        if (_movieService == null)
        {
            return StatusCode(500, new ApiResponse<MovieDTO>
            {
                StatusCode = 500,
                Message = "Movie service is not available",
                Data = null
            });
        }
        var removedMovie = await _movieService.RemoveMovieAsync(title);
        if (removedMovie != null)
        {
            var response = new ApiResponse<MovieDTO>
            {
                StatusCode = 200,
                Message = "Movie removed successfully",
                Data = removedMovie.ToDto()
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse<MovieDTO>
            {
                StatusCode = 404,
                Message = "Movie not found",
                Data = null
            };
            return NotFound(response);
        }
    }

    /// <summary>
    /// Updates the details of an existing movie. This is a placeholder implementation that simulates updating a movie. 
    /// In a real application, this would likely accept an identifier (such as a movie ID or title) 
    /// and a movie object with the updated details in the request, and then update the corresponding movie in a database or another data source.
    /// </summary>
    /// <param name="title">The title of the movie to update.</param>
    /// <param name="updatedMovie">The movie object containing the updated details.</param>
    /// <returns>The updated movie object if found; otherwise, null.</returns>
    [HttpPut("UpdateMovie")]
    public async Task<IActionResult> UpdateMovieAsync(string title, UpdateMovieDTO updatedMovie)
    {
        if (_movieService == null)
        {
            return StatusCode(500, new ApiResponse<MovieDTO>
            {
                StatusCode = 500,
                Message = "Movie service is not available",
                Data = null
            });
        }
        var updatedMovieResult = await _movieService.UpdateMovieAsync(title, updatedMovie.ToDomain());
        if (updatedMovieResult != null)
        {
            var response = new ApiResponse<MovieDTO>
            {
                StatusCode = 200,
                Message = "Movie updated successfully",
                Data = updatedMovieResult.ToDto()
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse<UpdateMovieDTO>
            {
                StatusCode = 404,
                Message = "Movie not found",
                Data = null
            };
            return NotFound(response);
        }
    }
}