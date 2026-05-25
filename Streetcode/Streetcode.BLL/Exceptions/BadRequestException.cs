namespace Streetcode.BLL.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message)
            : base(message, 400)
        {
        }
    }
}
