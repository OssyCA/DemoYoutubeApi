using DemoYoutubeApi.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.EntityFrameworkCore;

public class YouTubeAuthService(YoutubeDbContext db, IConfiguration config)
{
    public async Task<YouTubeService> GetYouTubeServiceAsync()
    {
        // ✅ Hämta senaste token från databasen
        var userToken = await db.UserTokens
            .OrderByDescending(u => u.CreatedAt)
            .FirstOrDefaultAsync();

        if (userToken == null)
            throw new Exception("Ingen token hittad! Kör inloggningen först.");

        var tokenResponse = new TokenResponse
        {
            AccessToken = userToken.AccessToken,
            RefreshToken = userToken.RefreshToken
        };

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = config["Google:ClientId"],
                ClientSecret = config["Google:ClientSecret"]
            }
        });

        var credential = new UserCredential(flow, "user", tokenResponse);

        return new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "KursAppen"
        });
    }
}
