using FluentResults;
using MediatR;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;

namespace Streetcode.BLL.MediatR.Email
{
    public class SendEmailHandler : IRequestHandler<SendEmailCommand, Result<Unit>>
    {
        private const string RecipientEmail = "streetcodeua@gmail.com";
        private const string EmailSubject = "FeedBack";

        private readonly IEmailPublisher _emailPublisher;
        private readonly ILoggerService _logger;

        public SendEmailHandler(IEmailPublisher emailPublisher, ILoggerService logger)
        {
            _emailPublisher = emailPublisher;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var message = new EmailMessageContract
                {
                    To = new List<string> { RecipientEmail },
                    From = request.Email.From,
                    Subject = EmailSubject,
                    Content = request.Email.Content,
                };

                await _emailPublisher.PublishAsync(message, cancellationToken);

                return Result.Ok(Unit.Value);
            }
            catch (Exception ex)
            {
                const string errorMsg = "Failed to publish email message to RabbitMQ.";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg).CausedBy(ex));
            }
        }
    }
}