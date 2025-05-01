using System.ComponentModel.DataAnnotations;

namespace DemoYoutubeApi.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public string? PlaylistId { get; set; } 
    }
}
