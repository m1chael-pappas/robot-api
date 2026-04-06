using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_api.Models;
using robot_api.Persistence;

namespace robot_api.Authentication;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Response.Headers.WWWAuthenticate = @"Basic realm=""Access to the robot controller.""";

        var authHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
        {
            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
        }

        var base64String = authHeader.Substring("Basic ".Length).Trim();
        var credentialBytes = Convert.FromBase64String(base64String);
        var credentials = Encoding.UTF8.GetString(credentialBytes);
        var parts = credentials.Split(':', 2);

        if (parts.Length != 2)
        {
            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
        }

        var email = parts[0];
        var password = parts[1];

        var user = UserDataAccess.GetUserByEmail(email);
        if (user == null)
        {
            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
        }

        var hasher = new PasswordHasher<UserModel>();
        var pwVerificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (pwVerificationResult == PasswordVerificationResult.Failed)
        {
            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed."));
        }

        var claims = new[]
        {
            new Claim("name", $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role ?? "User"),
        };

        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(authTicket));
    }
}
