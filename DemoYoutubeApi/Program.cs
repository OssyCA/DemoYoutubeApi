
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace DemoYoutubeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var apiKey = builder.Configuration["youtubeApiKey"];

            // Register the YouTubeService with the API key from configuration as a singleton

            builder.Services.AddSingleton(async _ =>
            {
                using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);

                var cred = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "user",                          // identifierar den som loggar in
                    CancellationToken.None,
                    new FileDataStore("YT.Auth.Store", true));  // sparar refresh-token lokalt

                return new YouTubeService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = cred,
                    ApplicationName = "KursAppen"
                });
            });

            builder.Services.AddControllers();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var apiKey = builder.Configuration["youtubeApiKey"];

            builder.Services.AddSingleton(provider =>
            {
                return new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey,
                    ApplicationName = "KursAppen"
                });
            });


            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
