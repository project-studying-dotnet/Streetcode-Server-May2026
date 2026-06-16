using System.Diagnostics.CodeAnalysis;

namespace Streetcode.Shared.Web.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message)
            : base(message, 400)
        {
        }
    }
}
