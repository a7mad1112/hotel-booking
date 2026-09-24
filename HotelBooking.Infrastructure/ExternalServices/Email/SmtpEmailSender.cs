using HotelBooking.Application.Common.Email;
using HotelBooking.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HotelBooking.Infrastructure.ExternalServices.Email;

public sealed class SmtpEmailSender : IEmailSender, IScopedService
{
    private readonly EmailOptions _options;

    public SmtpEmailSender(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            throw new InvalidOperationException("Email SMTP host is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.FromEmail))
        {
            throw new InvalidOperationException("Email sender address is not configured.");
        }

        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));

        mimeMessage.To.Add(MailboxAddress.Parse(message.To));

        mimeMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = message.Body
        };

        if (message.Attachment is not null)
        {
            bodyBuilder.Attachments.Add(
                message.Attachment.FileName,
                message.Attachment.Content,
                ContentType.Parse(message.Attachment.ContentType));
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        var secureSocketOptions = _options.UseSsl
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        await client.ConnectAsync(_options.Host, _options.Port, secureSocketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        }

        await client.SendAsync(mimeMessage, cancellationToken);

        await client.DisconnectAsync(true, cancellationToken);
    }
}