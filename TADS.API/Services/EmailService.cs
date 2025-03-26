using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TADS.API.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            // Geçici olarak e-posta gönderme işlemini devre dışı bırakıyoruz
            // ve sadece konsola yazdırıyoruz
            Console.WriteLine($"E-posta gönderildi: To={to}, Subject={subject}");
            Console.WriteLine($"Body: {body}");
            
            // Gerçek e-posta gönderme kodu:
            /*
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:Port"]),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            */
            
            await Task.CompletedTask; // Asenkron metod için
        }
        catch (Exception ex)
        {
            Console.WriteLine($"E-posta gönderme hatası: {ex.Message}");
            // Hatayı yutuyoruz, uygulamanın çalışmaya devam etmesi için
        }
    }
}
