using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly YouTubeAuthService _ytAuth;

    public VideoController(YouTubeAuthService ytAuth)
    {
        _ytAuth = ytAuth;
    }

    [HttpPost("set-unlisted/{videoId}")]
    public async Task<IActionResult> SetUnlisted(string videoId)
    {
        // 1) Hämta en autentiserad YouTubeService
        var yt = await _ytAuth.GetYouTubeServiceAsync();

        // 2) Bygg ett "Video"-objekt med bara Id + Status
        var vid = new Google.Apis.YouTube.v3.Data.Video
        {
            Id = videoId,
            Status = new Google.Apis.YouTube.v3.Data.VideoStatus
            {
                PrivacyStatus = "unlisted"
            }
        };

        // 3) Kör Update *endast* med part = "status"
        var updateReq = yt.Videos.Update(vid, "status");
        var updateResp = await updateReq.ExecuteAsync();

        return Ok(new
        {
            message = $"Video {updateResp.Id} är nu {updateResp.Status.PrivacyStatus}"
        });
    }

    

}
