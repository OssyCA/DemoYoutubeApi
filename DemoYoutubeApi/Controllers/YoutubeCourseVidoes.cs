using DemoYoutubeApi.CoursesData;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DemoYoutubeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YoutubeCourseVidoes(YouTubeService yt, ICourseRepo repo, IMemoryCache cache) : Controller
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
        [HttpGet("{courseId:int}")]
        public async Task<IActionResult> GetVideosCourseID(int courseId)
        {
            // 3a. Hämta PlaylistId
            var playlistId = await repo.GetPlaylistIdAsync(courseId);
            if (playlistId is null)
                return NotFound("Kurs saknar spellista");

            // 3b. Cache-nyckeln baseras på spellistan
            var cacheKey = $"playlist:{playlistId}";

            // 3c. Försök läsa från cache (30 minuters sliding window)
            if (cache.TryGetValue(cacheKey, out List<object> cachedVideos))
                return Ok(cachedVideos);

            // 3d. Ingen cache – loopa igenom alla sidor
            var request = yt.PlaylistItems.List("snippet,contentDetails");
            request.PlaylistId = playlistId;
            request.MaxResults = 50;

            var videos = new List<object>();
            string? next = null;

            do
            {
                request.PageToken = next;
                var resp = await request.ExecuteAsync();

                videos.AddRange(resp.Items.Select(i => new
                {
                    title = i.Snippet.Title,
                    videoId = i.ContentDetails.VideoId,
                    url = $"https://youtu.be/{i.ContentDetails.VideoId}"
                }));

                next = resp.NextPageToken;
            }
            while (next is not null);

            // 3e. Lägg resultatet i cache
            var opts = new MemoryCacheEntryOptions()
                           .SetSlidingExpiration(TimeSpan.FromMinutes(30));
            cache.Set(cacheKey, videos, opts);

            return Ok(videos);
        }
    }
}
