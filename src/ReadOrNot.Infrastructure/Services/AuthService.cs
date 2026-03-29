using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ReadOrNot.Application.Common;
using ReadOrNot.Application.DTOs.Account;
using ReadOrNot.Application.DTOs.Auth;
using ReadOrNot.Application.Interfaces;
using ReadOrNot.Application.Options;
using ReadOrNot.Infrastructure.Identity;

namespace ReadOrNot.Infrastructure.Services;

internal sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    IAccessTokenService accessTokenService,
    IClock clock,
    IOptions<FrontendOptions> frontendOptions,
    IHostEnvironment hostEnvironment) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await userManager.Users.SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail.ToUpperInvariant(), cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException("An account with this email address already exists.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
            DisplayName = request.DisplayName.Trim(),
            CreatedAtUtc = clock.UtcNow,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw BuildValidationException(result);
        }

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            throw new ValidationAppException("Login failed.", new Dictionary<string, string[]> { ["email"] = ["Invalid email or password."] });
        }

        var isValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isValid)
        {
            throw new ValidationAppException("Login failed.", new Dictionary<string, string[]> { ["password"] = ["Invalid email or password."] });
        }

        return CreateAuthResponse(user);
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return new ForgotPasswordResponse(true, null, null);
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        if (!hostEnvironment.IsDevelopment())
        {
            return new ForgotPasswordResponse(true, null, null);
        }

        var resetUrl = $"{frontendOptions.Value.BaseUrl.TrimEnd('/')}/#/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(encodedToken)}";
        return new ForgotPasswordResponse(true, encodedToken, resetUrl);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return;
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
        if (!result.Succeeded)
        {
            throw BuildValidationException(result);
        }
    }

    private AuthResponse CreateAuthResponse(ApplicationUser user)
    {
        var token = accessTokenService.Create(user.Id, user.Email!, user.DisplayName);
        return new AuthResponse(
            token.Token,
            token.ExpiresAtUtc,
            new AccountProfileResponse(user.Id, user.Email!, user.DisplayName, user.TimeZone, user.CreatedAtUtc));
    }

    private static ValidationAppException BuildValidationException(IdentityResult result)
    {
        var errors = result.Errors
            .GroupBy(error => error.Code, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray(), StringComparer.OrdinalIgnoreCase);

        return new ValidationAppException("Validation failed.", errors);
    }
}
