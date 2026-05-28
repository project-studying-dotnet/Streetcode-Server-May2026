using System.Diagnostics.CodeAnalysis;

namespace Streetcode.BLL.Exceptions
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
