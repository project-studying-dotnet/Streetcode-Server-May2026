using System.Diagnostics.CodeAnalysis;

namespace Streetcode.Shared.Web.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message)
            : base(message, 403)
        {
        }
    }
}
