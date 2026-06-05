namespace Ggio.BikeSherpa.Backend.Infrastructure.Mail;

public class PostmarkOptions
{
    public const string SectionName = "Postmark";
    public string ServerToken { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
}
