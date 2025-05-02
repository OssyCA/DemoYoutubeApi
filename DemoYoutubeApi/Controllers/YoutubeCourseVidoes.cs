using DemoYoutubeApi.CoursesData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/[controller]")]
public class YoutubeCourseVideosController(
    YouTubeAuthService auth,
    ICourseRepo repo,
    IMemoryCache cache) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetVideos()
    {
        // Hämta en autentiserad YouTubeService
        var yt = await auth.GetYouTubeServiceAsync();

        var request = yt.PlaylistItems.List("snippet,contentDetails");
        request.PlaylistId = "PLmHPezMl4BEd3nTPmpe3a8FjRilJJLQn4";
        request.MaxResults = 50;

        var response = await request.ExecuteAsync();

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
        var playlistId = await repo.GetPlaylistIdAsync(courseId);
        if (playlistId is null)
            return NotFound("Kurs saknar spellista");

        var cacheKey = $"playlist:{playlistId}";
        if (cache.TryGetValue(cacheKey, out List<object> cached))
            return Ok(cached);

        var yt = await auth.GetYouTubeServiceAsync();

        var request = yt.PlaylistItems.List("snippet,contentDetails");
        request.PlaylistId = playlistId;
        request.MaxResults = 50;

        var videos = new List<object>();
        string next = null;

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
        } while (next != null);

        cache.Set(cacheKey, videos,
            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30)));

        return Ok(videos);
    }
}
