using Core.Data;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        public async Task<bool> RegisterUser(RegisterUserRequestModel user)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email,
                PhoneNumber = user.Phone,
            };

            var result = await _userManager.CreateAsync(applicationUser, user.Password);

            if (result.Succeeded)
            {
                return true;
            }

            return false;
        }

        public async Task<SignInResponseModel> SignIn(SignInRequestModel request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            var isSucceeded = await _userManager.CheckPasswordAsync(user, request.Password);

            if (isSucceeded)
            {
                var claims = new Claim[]
                {
                    new Claim("username", request.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Value.Issuer,
                    audience: _jwtSettings.Value.Issuer,
                    claims: claims,
                    expires: DateTime.Now.AddDays(1), 
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                return new SignInResponseModel
                {
                    User = user,
                    Message = tokenAsString,
                    ExpiresAt = token.ValidTo,
                    IsSuccess = true
                };
            }

            return new SignInResponseModel();
        }
    }
}
