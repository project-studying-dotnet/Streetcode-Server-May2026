using Streetcode.BLL.MediatR.Interface;
using Streetcode.BLL.Validators;

namespace Streetcode.XUnitTest.BLL.Validators
{
    public class TestStreetcodeQuery : IHasStreetcodeId
    {
        public int StreetcodeId { get; set; }
    }
}
