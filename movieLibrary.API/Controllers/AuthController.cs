using Microsoft.AspNetCore.Mvc;
using movieLibraryService.Models.DTOs.UserDTOs;
using movieLibraryService.Services.Security;
using movieLibraryService.Models.Response;

namespace movieLibraryAPI.Controllers;

public class AuthController : ControllerBase
{
    private readonly LoginService _loginService;

    public AuthController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO registerUserDTO, CancellationToken ct)
    {
        var result = await _loginService.RegisterUserAsync(registerUserDTO, ct);
        return ToHttpResult(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO, CancellationToken ct)
    {
        var result = await _loginService.LoginAsync(userLoginDTO, ct);
        return ToHttpResult(result);
    }

    [HttpPost("password-recovery/request")]
    public async Task<IActionResult> RequestPasswordRecovery([FromBody] PasswordRecoveryRequestDTO requestDTO, CancellationToken ct)
    {
        var result = await _loginService.RequestPasswordRecoveryAsync(requestDTO, ct);
        return ToHttpResult(result);
    }

    [HttpPost("password-recovery/confirm")]
    public async Task<IActionResult> ConfirmPasswordRecovery([FromBody] PasswordRecoveryConfirmDTO confirmDTO, CancellationToken ct)
    {
        var result = await _loginService.ConfirmPasswordRecoveryAsync(confirmDTO, ct);
        return ToHttpResult(result);
    }

    private IActionResult ToHttpResult<T>(ApiResponse<T> apiResponse)
    {
        if (apiResponse is null) return StatusCode(500, "Unexpected null response.");
        if (apiResponse.Success) return Ok(apiResponse);

        return apiResponse.StatusCode switch
        {
            400 => BadRequest(apiResponse),
            401 => Unauthorized(apiResponse),
            404 => NotFound(apiResponse),
            409 => Conflict(apiResponse),
            _ => StatusCode(apiResponse.StatusCode <= 0 ? 500 : apiResponse.StatusCode, apiResponse)
        };
    }
}