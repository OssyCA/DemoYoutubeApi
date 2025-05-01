using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;

namespace DemoYoutubeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YoutubeCourseVidoes(YouTubeService yt) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetVideos()
        {
            var request = yt.PlaylistItems.List("snippet,contentDetails");
            request.PlaylistId = "PLmHPezMl4BEd3nTPmpe3a8FjRilJJLQn4";   // ← ditt ID
            request.MaxResults = 50;

            var response = await request.ExecuteAsync();

            // Projicera till en enkel DTO
            var videos = response.Items.Select(i => new
            {
                title = i.Snippet.Title,
                videoId = i.ContentDetails.VideoId,
                url = $"https://youtu.be/{i.ContentDetails.VideoId}"
            });

            return Ok(videos);
        }
    }
}
