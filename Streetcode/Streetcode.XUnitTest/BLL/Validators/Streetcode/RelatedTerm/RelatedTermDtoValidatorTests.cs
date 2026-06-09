using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Validators.Streetcode.RelatedTerm;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedTerm
{
    public class RelatedTermDtoValidatorTests
    {
        private readonly RelatedTermDtoValidator _validator;

        public RelatedTermDtoValidatorTests()
        {
            _validator = new RelatedTermDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Word_Is_Empty(string? invalidWord)
        {
            var dto = CreateValidDto();
            dto.Word = invalidWord!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Word);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_TermId_Is_Less_Or_Equal_To_Zero(int invalidTermId)
        {
            var dto = CreateValidDto();
            dto.TermId = invalidTermId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TermId);
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Less_Than_Zero()
        {
            var dto = CreateValidDto();
            dto.Id = -1;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static RelatedTermDTO CreateValidDto()
        {
            return new RelatedTermDTO
            {
                Id = 0,
                TermId = 1,
                Word = "Термін"
            };
        }
    }
}
