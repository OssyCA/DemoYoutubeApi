using System.ComponentModel.DataAnnotations;

namespace DemoYoutubeApi.Models
{
    public class UserToken
    {
        [Key]
        public int Id { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
