using movieLibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;
using movieLibraryAPI.Models.Response;
using movieLibraryAPI.Models.DTOs;
using movieLibraryAPI.Mappings;

namespace movieLibraryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TvShowApiController : ControllerBase
{
    private readonly TvShowService? _tvShowService;

    public TvShowApiController(TvShowService? tvShowService)
    {
        _tvShowService = tvShowService;
    }

    /// <summary>
    /// Retrieves a list of TV shows. This is a placeholder implementation that returns a static list of TV show titles. 
    /// In a real application, this would likely query a database or another data source to retrieve the TV show information.
    /// </summary>
    /// <returns></returns>
    [HttpGet("TvShows")]
    public async Task<IActionResult> GetShows()
    {
        if (_tvShowService == null)
        {
            return StatusCode(500, new ApiResponse<List<TvShowDTO>>
            {
                StatusCode = 500,
                Message = "TV show service is not available",
                Data = null
            });
        }

        var tvShows = await _tvShowService.GetAllTvShowsAsync();
        return Ok(new ApiResponse<List<TvShowDTO>>
        {
            StatusCode = 200,
            Message = "TV shows retrieved successfully",
            Data = tvShows.Select(tvShow => tvShow.ToDto()).ToList()
        });
    }

    /// <summary>
    /// Adds a new TV show to the collection. This is a placeholder implementation that simulates adding a TV show. 
    /// In a real application, this would likely accept a TV show object in the request body and save it to a database or another data source.
    /// </summary>
    /// <param name="newTvShow">The TV show object containing the details of the new TV show.</param>
    /// <returns>The created TV show object.</returns>
    [HttpPost("AddTvShow")]
    public async Task<IActionResult> AddTvShow(CreateTvShowDTO newTvShow)
    {
        if (_tvShowService == null)
        {
            return StatusCode(500, new ApiResponse<CreateTvShowDTO>
            {
                StatusCode = 500,
                Message = "TV show service is not available",
                Data = null
            });
        }
        var domainModel = newTvShow.ToDomain();
        var createdTvShow = await _tvShowService.AddTvShowAsync(domainModel);
        var response = new ApiResponse<TvShowDTO>
        {
            StatusCode = 201,
            Message = "TV show added successfully",
            Data = createdTvShow.ToDto()
        };
        return CreatedAtAction(nameof(GetShows), new { title = createdTvShow.Title }, response);
    }

    /// <summary>
    /// Removes a TV show from the collection. This is a placeholder implementation that simulates removing a TV show. 
    /// In a real application, this would likely accept an identifier (such as a TV show ID or title) in the request and remove the corresponding TV show from a database or another data source.
    /// </summary>
    /// <param name="title">The title of the TV show to remove.</param>
    /// <returns>The removed TV show object if found; otherwise, null.</returns>
    [HttpDelete("RemoveTvShow")]
    public async Task<IActionResult> RemoveTvShow(string title)
    {
        if (_tvShowService == null)
        {
            return StatusCode(500, new ApiResponse<TvShowDTO>
            {
                StatusCode = 500,
                Message = "TV show service is not available",
                Data = null
            });
        }
        var removedTvShow = await _tvShowService.RemoveTvShowAsync(title);
        if (removedTvShow != null)
        {
            var response = new ApiResponse<TvShowDTO>
            {
                StatusCode = 200,
                Message = "TV show removed successfully",
                Data = removedTvShow.ToDto()
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse<TvShowDTO>
            {
                StatusCode = 404,
                Message = "TV show not found",
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
    /// <param name="title">The title of the TV show to update.</param>
    /// <param name="updatedTvShow">The TV show object containing the updated details.</param>
    /// <returns>The updated TV show object if found; otherwise, null.</returns>
    [HttpPut("UpdateTvShow")]
    public async Task<IActionResult> UpdateTvShow(string title, UpdateTvShowDTO updatedTvShow)
    {
        if (_tvShowService == null)
        {
            return StatusCode(500, new ApiResponse<TvShowDTO>
            {
                StatusCode = 500,
                Message = "TV show service is not available",
                Data = null
            });
        }
        var domainModel = updatedTvShow.ToDomain();
        var updatedTvShowResult = await _tvShowService.UpdateTvShowAsync(title, domainModel);
        if (updatedTvShowResult != null)
        {
            var response = new ApiResponse<TvShowDTO>
            {
                StatusCode = 200,
                Message = "TV show updated successfully",
                Data = updatedTvShowResult.ToDto()
            };
            return Ok(response);
        }
        else
        {
            var response = new ApiResponse<TvShowDTO>
            {
                StatusCode = 404,
                Message = "TV show not found",
                Data = null
            };
            return NotFound(response);
        }
    }
}