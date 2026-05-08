using Microsoft.AspNetCore.Mvc;
using movieLibrary.Models;
using movieLibrary.Models.Movies;
using movieLibrary.Models.TvShows;
using movieLibrary.Services;

namespace movieLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    private readonly MovieService _movieService;
    private readonly TvShowService _tvShowService;
    /// <summary>
    /// Retrieves a list of TV shows. This is a placeholder implementation that returns a static list of TV show titles. 
    /// In a real application, this would likely query a database or another data source to retrieve the TV show information.
    /// </summary>
    /// <returns></returns>
    [HttpGet("TvShows")]
    public async Task<IActionResult> GetShows()
    {
        var tvShows = await _tvShowService.GetAllTvShowsAsync();
        return Ok(new ApiResponse
        {
            StatusCode = 200,
            Message = "Tv shows retrieved successfully",
            Data = tvShows
        });
    }

    /// <summary>
    /// Retrieves a list of movies. This is a placeholder implementation that returns a static list of movie titles. 
    /// In a real application, this would likely query a database or another data source to retrieve the movie information.
    /// </summary>
    /// <returns></returns>
    [HttpGet("Movies")]
    public async Task<IActionResult> GetMovies()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        var response = new ApiResponse
        {
            StatusCode = 200,
            Message = "Movies retrieved successfully",
            Data = movies
        };
        return Ok(response);
    }

    /// <summary>
    /// Adds a new movie to the collection. This is a placeholder implementation that simulates adding a movie. 
    /// In a real application, this would likely accept a movie object in the request body and save it to a database or another data source.
    /// </summary>
    /// <returns></returns>
    [HttpPost("AddMovie")]
    public async Task<IActionResult> AddMovie(Movie newMovie)
    {
        var createdMovie = await _movieService.AddMovieAsync(new Movie
        {
            Title = newMovie.Title,
            ReleaseYear = newMovie.ReleaseYear,
            Description = newMovie.Description,
            MovieGenre = newMovie.MovieGenre
        });
        var response = new ApiResponse
        {
            StatusCode = 201,
            Message = "Movie added successfully",
            Data = createdMovie
        };
        return CreatedAtAction(nameof(GetMovies), new { title = createdMovie.Title }, response);
    }

    /// <summary>
    /// Adds a new TV show to the collection. This is a placeholder implementation that simulates adding a TV show. 
    /// In a real application, this would likely accept a TV show object in the request body and save it to a database or another data source.
    /// </summary>
    /// <returns></returns>
    [HttpPost("AddTvShow")]
    public async Task<IActionResult> AddTvShow(TvShow newTvShow)
    {
        var createdTvShow = await _tvShowService.AddTvShowAsync(newTvShow);
        return CreatedAtAction(nameof(GetShows), new { title = createdTvShow.Title }, new ApiResponse
        {
            StatusCode = 201,
            Message = "Tv show added successfully",
            Data = createdTvShow
        });
    }

    /// <summary>
    /// Removes a movie from the collection. This is a placeholder implementation that simulates removing a movie. 
    /// In a real application, this would likely accept an identifier (such as a movie ID or title) in the request and remove the corresponding movie from a database or another data source.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("RemoveMovie")]
    public async Task<IActionResult> RemoveMovie(string title)
    {
        var removedMovie = await _movieService.RemoveMovieAsync(title);
        if (removedMovie != null)
        {
            var response = new ApiResponse
            {
                StatusCode = 200,
                Message = "Movie removed successfully",
                Data = removedMovie
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse
            {
                StatusCode = 404,
                Message = "Movie not found",
                Data = null
            };
            return NotFound(response);
        }
    }

    /// <summary>
    /// Removes a TV show from the collection. This is a placeholder implementation that simulates removing a TV show. 
    /// In a real application, this would likely accept an identifier (such as a TV show ID or title) in the request and remove the corresponding TV show from a database or another data source.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("RemoveTvShow")]
    public async Task<IActionResult> RemoveTvShow(string title)
    {
        var removedTvShow = await _tvShowService.RemoveTvShowAsync(title);
        if (removedTvShow != null)
        {
            var response = new ApiResponse
            {
                StatusCode = 200,
                Message = "Tv show removed successfully",
                Data = removedTvShow
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse
            {
                StatusCode = 404,
                Message = "Tv show not found",
                Data = null
            };
            return NotFound(response);
        }
    }

    /// <summary>
    /// Updates the details of an existing movie. This is a placeholder implementation that simulates updating a movie. 
    /// In a real application, this would likely accept an identifier (such as a movie ID or title) and a movie object with the updated details in the request, 
    /// and then update the corresponding movie in a database or another data source.
    /// </summary>
    /// <returns></returns>
    [HttpPut("UpdateMovie")]
    public async Task<IActionResult> UpdateMovieAsync(string title, Movie updatedMovie)
    {
        var updatedMovieResult = await _movieService.UpdateMovieAsync(title, updatedMovie);
        if (updatedMovieResult != null)
        {
            var response = new ApiResponse
            {
                StatusCode = 200,
                Message = "Movie updated successfully",
                Data = updatedMovieResult
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse
            {
                StatusCode = 404,
                Message = "Movie not found",
                Data = null
            };
            return NotFound(response);
        }
    }

    /// <summary>
    /// Updates the details of an existing TV show. This is a placeholder implementation that simulates updating a TV show. 
    /// In a real application, this would likely accept an identifier (such as a TV show ID or title) and a TV show object with the updated details in the request, 
    /// and then update the corresponding TV show in a database or another data source.
    /// </summary>
    /// <returns></returns>
    [HttpPut("UpdateTvShow")]
    public async Task<IActionResult> UpdateTvShow(string title, TvShow updatedTvShow)
    {
        var updatedTvShowResult = await _tvShowService.UpdateTvShowAsync(title, updatedTvShow);
        if (updatedTvShowResult != null)
        {
            var response = new ApiResponse
            {
                StatusCode = 200,
                Message = "Tv show updated successfully",
                Data = updatedTvShowResult
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse
            {
                StatusCode = 404,
                Message = "Tv show not found",
                Data = null
            };
            return NotFound(response);
        }
    }
}