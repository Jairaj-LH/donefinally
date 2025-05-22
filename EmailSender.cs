using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Here you can integrate your email service or just log to console for now
        Console.WriteLine($"Sending email to: {email}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {htmlMessage}");
        return Task.CompletedTask; // Simulating sending email
    }
}
