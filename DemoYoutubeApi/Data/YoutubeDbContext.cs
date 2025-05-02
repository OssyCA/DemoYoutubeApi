using DemoYoutubeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoYoutubeApi.Data
{
    public class YoutubeDbContext(DbContextOptions<YoutubeDbContext> options) : DbContext(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
    }
}
