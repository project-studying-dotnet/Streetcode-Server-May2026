using System.Diagnostics.CodeAnalysis;

namespace Streetcode.BLL.Exceptions
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
