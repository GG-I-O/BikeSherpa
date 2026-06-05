namespace Ggio.BikeSherpa.Backend.Infrastructure.Mail;

public interface IMailSender
{
     Task SendEmailAsync(string email, string subject, string message);
}

public class MailSender : IMailSender
{
     public Task SendEmailAsync(string email, string subject, string message) => Task.CompletedTask;
}
