namespace DemoYoutubeApi.CoursesData
{
    public interface ICourseRepo
    {
        Task<string?> GetPlaylistIdAsync(int courseId);
    }
}
