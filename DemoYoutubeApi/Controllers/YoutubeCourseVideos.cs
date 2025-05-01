using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;

namespace DemoYoutubeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YoutubeCourseVideos(YouTubeService yt) : Controller
    {
        /// <summary>
        /// Returnerar alla spellistor som tillhör inloggat Google-konto (även olistade).
        /// GET api/playlist/mine
        /// </summary>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyPlaylists()
        {
            var req = yt.Playlists.List("id,snippet,status");
            req.Mine = true;
            req.MaxResults = 50;

            var resp = await req.ExecuteAsync();

            var result = resp.Items.Select(p => new
            {
                p.Id,
                Title = p.Snippet.Title,
                Privacy = p.Status.PrivacyStatus
            });

            return Ok(result);
        }

        /// <summary>
        /// Returnerar alla videor i en viss spellista.
        /// GET api/playlist/{playlistId}/videos
        /// </summary>
        [HttpGet("{playlistId}/videos")]
        public async Task<IActionResult> GetVideos(string playlistId)
        {
            var req = yt.PlaylistItems.List("snippet,contentDetails");
            req.PlaylistId = playlistId;
            req.MaxResults = 50;

            var resp = await req.ExecuteAsync();

            var videos = resp.Items.Select(v => new
            {
                v.Snippet.Title,
                v.ContentDetails.VideoId,
                Url = $"https://youtu.be/{v.ContentDetails.VideoId}"
            });

            return Ok(videos);
        }
    }

}

