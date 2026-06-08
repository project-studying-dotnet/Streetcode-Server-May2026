using Streetcode.BLL.Contracts;

namespace Streetcode.BLL.Interfaces.Email;

public interface IEmailPublisher
{
    Task PublishAsync(
        EmailMessageContract message,
        CancellationToken cancellationToken = default);
}