using Microsoft.AspNetCore.Mvc;
using movieLibraryService.Models.DTOs.UserDTOs;
using movieLibraryService.Services.Security;

namespace movieLibraryAPI.Controllers;

public class AuthController : ControllerBase
{
    private readonly LoginService _loginService;

    public AuthController(LoginService loginService)
    {
        _loginService = loginService;
    }
}