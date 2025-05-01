using DemoYoutubeApi.Models;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DemoYoutubeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YoutubeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Add constructor to inject IConfiguration
        public YoutubeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GetChannelVideo(string? pagetoken = null, int maxResult = 50)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = _configuration.GetValue<string>("YoutubeApiKey"),
                ApplicationName = "YoutubeAPI"
            });

            var searchRequest = youtubeService.Search.List("snippet"); // Get snippet of video, title, thuimbnail, video id, date, discription

            searchRequest.ChannelId = "UC0QHWhjbe5fGJEPz3sVb6nw";
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchRequest.MaxResults = maxResult;
            searchRequest.PageToken = pagetoken;
            
            var searchResponse = await searchRequest.ExecuteAsync();

            var videoList = searchResponse.Items.Select(i => new VideoDetails
            {
                Title = i.Snippet.Title,
                Link = $"https://www.youtube.com/watch?v={i.Id.VideoId}",
                Thumbnail = i.Snippet.Thumbnails.Medium.Url,
                Published = i.Snippet.PublishedAtDateTimeOffset
            }).OrderByDescending(video => video.Published).ToList();

            var response = new YoutubeResponse
            {
                Videos = videoList,
                NextPageToken = searchResponse.NextPageToken,
                PrevPageToken = searchResponse.PrevPageToken,
            };

            return Ok(response);
        }
    }
}
