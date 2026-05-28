using System.Diagnostics.CodeAnalysis;

namespace Streetcode.BLL.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message)
            : base(message, 404)
        {
        }
    }
}
