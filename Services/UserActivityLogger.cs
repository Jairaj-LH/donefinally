using charac.Data;
using charac.Models;

namespace charac.Services
{
    public interface IUserActivityLogger
    {
        Task LogAsync(string userId, string action, string description);
    }

    public class UserActivityLogger : IUserActivityLogger
    {
        private readonly ApplicationDbContext _context;

        public UserActivityLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, string action, string description)
        {
            var log = new UserActivityHistory
            {
                UserId = userId,
                Action = action,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            _context.UserActivityHistories.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
