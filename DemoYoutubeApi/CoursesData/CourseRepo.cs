using DemoYoutubeApi.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace DemoYoutubeApi.CoursesData
{
    public class CourseRepo(YoutubeDbContext db) : ICourseRepo
    {
        public async Task<string?> GetPlaylistIdAsync(int courseId)
        {
            return await db.Courses
                           .Where(c => c.CourseId == courseId)
                           .Select(c => c.PlaylistId)
                           .SingleOrDefaultAsync();
        }
    }
}
