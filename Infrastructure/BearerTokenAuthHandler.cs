using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SendSmsApi.Infrastructure;

public class BearerTokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public BearerTokenAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Čitaj Authorization header
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            return Task.FromResult(AuthenticateResult.Fail("Nedostaje Authorization header."));

        var headerValue = authHeader.ToString();
        if (!headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("Neispravan format tokena. Koristite: Bearer <token>"));

        var token = headerValue.Substring("Bearer ".Length).Trim();

        // Učitaj validne tokene iz appsettings.json
        var validTokens = _configuration.GetSection("ApiTokens").Get<List<string>>() ?? new List<string>();

        if (!validTokens.Contains(token))
            return Task.FromResult(AuthenticateResult.Fail("Neispravan token."));

        // Token je validan — napravi identity
        var claims = new[] { new Claim(ClaimTypes.Name, "ApiConsumer") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}