using Shared.DataTransferObjects.AuthDTOs;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Services.Abstraction
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<bool>> RegisterAsync(RegisterDTO registerData);
        Task<GenericResponse<bool>> RegisterDriverAsync(RegisterDriverDTO registerData);
        Task<GenericResponse<UserDTO>> LoginAsync(LoginDTO loginData);
        Task<GenericResponse<bool>> ChangePasswordAsync(string email, ChangePasswordDTO changePasswordData);
    }
}
