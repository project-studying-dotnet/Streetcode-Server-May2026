using System.Diagnostics.CodeAnalysis;

namespace Streetcode.Shared.Web.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message)
            : base(message, 401)
        {
        }
    }
}
