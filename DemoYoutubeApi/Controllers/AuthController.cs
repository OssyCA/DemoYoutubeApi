using DemoYoutubeApi.Data;
using DemoYoutubeApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config, YoutubeDbContext db) : ControllerBase
{

    // 1️⃣ Skicka användaren till Google för inloggning + samtycke
    [HttpGet("login")]
    public IActionResult Login()
    {
        var clientId = config["Google:ClientId"];
        var clientSecret = config["Google:ClientSecret"];
        var redirectUri = config["Google:RedirectUri"];

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            Scopes = new[] { YouTubeService.Scope.YoutubeForceSsl }
        });

        var url = flow.CreateAuthorizationCodeRequest(redirectUri).Build();
        return Redirect(url.ToString());
    }

    // 2️⃣ Google skickar tillbaka här med ?code=...
    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        var clientId = config["Google:ClientId"];
        var clientSecret = config["Google:ClientSecret"];
        var redirectUri = config["Google:RedirectUri"];

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            Scopes = new[] { YouTubeService.Scope.YoutubeForceSsl }
        });

        // Byt code mot tokens
        var tokenResponse = await flow.ExchangeCodeForTokenAsync(
            userId: "admin",
            code: code,
            redirectUri: redirectUri,
            taskCancellationToken: CancellationToken.None
        );

        // Spara i databasen (uppdatera eller lägg till)
        var existing = await db.UserTokens
            .FirstOrDefaultAsync(u => u.Email == "karriarpartner@gmail.com");

        if (existing == null)
        {
            db.UserTokens.Add(new UserToken
            {
                Email = "karriarpartner@gmail.com",
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.AccessToken = tokenResponse.AccessToken;
            existing.RefreshToken = tokenResponse.RefreshToken;
            existing.CreatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        return Ok(new
        {
            message = "Inloggning lyckades och token är sparad!",
            accessToken = tokenResponse.AccessToken,
            refreshToken = tokenResponse.RefreshToken
        });
    }
}
