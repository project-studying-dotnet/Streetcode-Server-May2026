using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.XUnitTest.BLL.Validators
{
    public class TestStreetcodeQuery : IHasStreetcodeId
    {
        public int StreetcodeId { get; set; }
    }
}
