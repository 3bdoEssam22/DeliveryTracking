using DeliveryTracking.Core.Entities.SecurityModule;
using DeliveryTracking.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DataTransferObjects.AuthDTOs;
using Shared.Messages;
using Shared.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliveryTracking.Services
{
    public class AuthenticationService(UserManager<DeliveryTrackingUser> _userManager, IConfiguration _configuration, IEmailService _emailService) : IAuthenticationService
    {
        public async Task<GenericResponse<bool>> RegisterAsync(RegisterDTO registerData)
        {
            var genericResponse = new GenericResponse<bool>();

            var emailExists = await _userManager.FindByEmailAsync(registerData.Email);
            if (emailExists is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status409Conflict;
                genericResponse.Message = "Email is used";
                return genericResponse;
            }
            var user = new DeliveryTrackingUser()
            {
                Email = registerData.Email,
                FullName = registerData.FullName,
                PhoneNumber = registerData.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UserName = registerData.Email
            };

            var result = await _userManager.CreateAsync(user, registerData.Password);

            if (result.Succeeded)
            {
                var email = new Email()
                {
                    To = registerData.Email,
                    Subject = $"Welcome {registerData.FullName} to our DeliveryTracking Application.",
                    Body = "A welcome message from DeliveryTracking Support, Login and enjoy our Services."
                };

                await _emailService.SendEmailAsync(email);
                await _userManager.AddToRoleAsync(user, "Customer");
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Account Created successfully";
                genericResponse.Data = true;
            }
            else
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = string.Join(" | ", result.Errors.Select(e => e.Description));
            }
            return genericResponse;
        }

        public async Task<GenericResponse<UserDTO>> LoginAsync(LoginDTO loginData)
        {
            var genericResponse = new GenericResponse<UserDTO>();

            var user = await _userManager.FindByEmailAsync(loginData.Email);

            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                genericResponse.Message = "Invalid Email or Password";
                return genericResponse;
            }

            if (!user.IsActive)
            {
                genericResponse.StatusCode = StatusCodes.Status403Forbidden;
                genericResponse.Message = "Account has been Deactivated.";
                return genericResponse;
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                genericResponse.Message = "Account is temporarily locked. Try again later.";
                return genericResponse;
            }

            var result = await _userManager.CheckPasswordAsync(user, loginData.Password);

            if (!result)
            {
                await _userManager.AccessFailedAsync(user);
                genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                genericResponse.Message = "Invalid Email or Password";
            }
            else
            {
                var days = int.Parse(_configuration["JwtOptions:ExpiryDays"] ?? "30");
                await _userManager.ResetAccessFailedCountAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var userClaims = await _userManager.GetClaimsAsync(user);
                bool mustChangePassword = userClaims.Any(c => c.Type == "MustChangePassword" && c.Value == "true");

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Login Successfully.";
                var expiresAt = DateTime.UtcNow.AddDays(days);
                genericResponse.Data = new UserDTO()
                {
                    Email = loginData.Email,
                    FullName = user.FullName,
                    Token = CreateToken(user, expiresAt, roles),
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    ExpiresAt = expiresAt,
                    MustChangePassword = mustChangePassword
                };
            }
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> RegisterDriverAsync(RegisterDriverDTO registerData)
        {
            var genericResponse = new GenericResponse<bool>();

            var emailExists = await _userManager.FindByEmailAsync(registerData.Email);
            if (emailExists is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status409Conflict;
                genericResponse.Message = "Email is used";
                return genericResponse;
            }

            var user = new DeliveryTrackingUser()
            {
                Email = registerData.Email,
                FullName = registerData.FullName,
                PhoneNumber = registerData.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UserName = registerData.Email
            };

            var password = $"Dr!v3r{Guid.NewGuid().ToString()[..8].ToUpper()}";
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var email = new Email()
                {
                    To = registerData.Email,
                    Subject = $"Welcome {registerData.FullName} to our DeliveryTracking Application.",
                    Body = $"To login: Use your email, Your temp password is: ({password})"
                };

                await _emailService.SendEmailAsync(email);
                await _userManager.AddToRoleAsync(user, "Driver");
                await _userManager.AddClaimAsync(user, new Claim("MustChangePassword", "true"));
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Driver is created successfully.";
                genericResponse.Data = true;
            }
            else
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = string.Join(" | ", result.Errors.Select(e => e.Description));
            }
            return genericResponse;

        }

        public async Task<GenericResponse<bool>> ChangePasswordAsync(string email, ChangePasswordDTO changePasswordData)
        {
            var genericResponse = new GenericResponse<bool>();

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User not found";
                return genericResponse;
            }

            if (!user.IsActive)
            {
                genericResponse.StatusCode = StatusCodes.Status403Forbidden;
                genericResponse.Message = "Account has been Deactivated.";
                return genericResponse;
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordData.CurrentPassword, changePasswordData.NewPassword);
            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = string.Join(" | ", result.Errors.Select(e => e.Description));
                return genericResponse;
            }

            var mustChangeClaim = (await _userManager.GetClaimsAsync(user))
                .FirstOrDefault(c => c.Type == "MustChangePassword" && c.Value == "true");

            if (mustChangeClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, mustChangeClaim);
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Password changed successfully.";
            genericResponse.Data = true;

            return genericResponse;
        }

        #region Token
        private string CreateToken(DeliveryTrackingUser user, DateTime duration, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.NameId, user.Id.ToString()!),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Name, user.FullName!),
            };

            foreach (var role in roles)
                claims.Add(new(ClaimTypes.Role, role));

            var SecretKey = _configuration["JwtOptions:SecretKey"];

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey!));

            var cred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                claims: claims,
                expires: duration,
                signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
        #endregion

    }
}