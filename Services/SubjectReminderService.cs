using charac.Data;
using charac.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class SubjectReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubjectReminderService> _logger;
    private readonly IEmailService _emailService;

    public SubjectReminderService(IServiceProvider serviceProvider, ILogger<SubjectReminderService> logger, IEmailService emailService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Running SubjectReminderService at {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Select only subjects where EmailSent == false
                var subjectsToEmail = await db.Subjects
                    .Include(s => s.User)
                    .Where(s => !s.EmailSent)
                    .ToListAsync(stoppingToken);

                foreach (var subject in subjectsToEmail)
                {
                    if (subject.User != null && !string.IsNullOrEmpty(subject.User.Email))
                    {
                        var email = subject.User.Email;
                        var subjectLine = "Reminder: You created a new subject!";
                        var message = $@"
                            <p>Hi {subject.User.UserName},</p>
                            <p>You recently created a subject titled <strong>{subject.SubName}</strong>.</p>
                            <p>Don’t forget to keep writing your story!</p>
                            <p>— Your Storytelling App</p>";

                        await _emailService.SendEmailAsync(email, subjectLine, message);

                        _logger.LogInformation("Email sent to {Email} for subject {SubjectName}", email, subject.SubName);

                        // Mark email as sent
                        subject.EmailSent = true;
                    }
                }

                // Save changes to mark subjects as emailed
                await db.SaveChangesAsync(stoppingToken);
            }

            // Wait before checking again — adjust timing as needed
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
