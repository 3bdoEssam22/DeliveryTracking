using DeliveryTracking.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects.AuthDTOs;
using Shared.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeliveryTracking.Api.Controllers
{
    public class AuthController(IAuthenticationService _authenticationService) : BaseApiController
    {

        //Post BaseUrl/api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<GenericResponse<bool>>> RegisterAsync([FromBody] RegisterDTO registerData)
        {
            var result = await _authenticationService.RegisterAsync(registerData);
            return HandleResponse(result);
        }

        //Post BaseUrl/api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<GenericResponse<UserDTO>>> LoginAsync([FromBody] LoginDTO loginData)
        {
            var result = await _authenticationService.LoginAsync(loginData);
            return HandleResponse(result);
        }

        //post BaseUrl/api/auth/register-driver
        [HttpPost("register-driver")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GenericResponse<bool>>> RegisterDriverAsync([FromBody] RegisterDriverDTO registerData)
        {
            var result = await _authenticationService.RegisterDriverAsync(registerData);
            return HandleResponse(result);
        }

        //post BaseUrl/api/auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<GenericResponse<bool>>> ChangePasswordAsync([FromBody] ChangePasswordDTO changePasswordData)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var result = await _authenticationService.ChangePasswordAsync(email, changePasswordData);
            return HandleResponse(result);
        }

    }
}
