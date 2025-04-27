namespace DemoYoutubeApi.Models
{
    public class YoutubeResponse
    {
        public List<VideoDetails> Videos { get; set; } = [];
        public string? NextPageToken { get; set; }
        public string? PrevPageToken { get; set; }

    }
}
