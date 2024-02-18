using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BebodhCrawler.Extensions
{
    public static class ControllerExtension
    {
        public static string GetCurrentUserId(this ControllerBase controller)
        {
            try
            {
                var claims = controller.User.Claims;
                var userId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userId == null) return string.Empty;
                return userId.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR:: GetCurrentUserId failed!");
                Console.WriteLine($"ERROR Message:: {ex.Message}");
                return string.Empty;
            }
        }

        public static string GetCurrentUserName(this ControllerBase controller)
        {
            try
            {
                var claims = controller.User.Claims;
                var username = claims.FirstOrDefault(x => x.Type == "username");
                if (username == null) return string.Empty;
                return username.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR:: GetCurrentUserName failed!");
                Console.WriteLine($"ERROR Message:: {ex.Message}");
                return string.Empty;
            }
        }

        public static string GetAccessToken(this ControllerBase controller)
        {
            try
            {
                var authorizationHeader = controller.HttpContext.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorizationHeader)) return string.Empty;
                return authorizationHeader.ToString().Replace("Bearer", "").Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR:: GetAccessToken failed!");
                Console.WriteLine($"ERROR Message:: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
