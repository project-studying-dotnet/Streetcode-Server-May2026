using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.Validators.Media.Art.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Art.GetById
{
    public class GetArtByIdValidatorTests
    {
        private readonly GetArtByIdValidator _validator;

        public GetArtByIdValidatorTests()
        {
            _validator = new GetArtByIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetArtByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetArtByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}