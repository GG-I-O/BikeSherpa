using Microsoft.Extensions.Options;
using PostmarkDotNet;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Mail;

public interface IMailSender
{
     Task SendEmailAsync(string email, string subject, string message);
}

public class MailSender(IOptions<PostmarkOptions> options) : IMailSender
{
     private readonly PostmarkOptions _options = options.Value;

     public async Task SendEmailAsync(string email, string subject, string message)
     {
          var client = new PostmarkClient(_options.ServerToken);
          var postmarkMessage = new PostmarkMessage
          {
               To = email,
               From = _options.FromEmail,
               TrackOpens = true,
               Subject = subject,
               HtmlBody = message,
               TextBody = message,
          };

          await client.SendMessageAsync(postmarkMessage);
     }
}
