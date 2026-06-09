using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.Streetcode;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Streetcode.Streetcode
{
    public class StreetcodeDtoValidatorTests
    {
        private readonly StreetcodeDtoValidator _validator;

        public StreetcodeDtoValidatorTests()
        {
            _validator = new StreetcodeDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var dto = CreateValidDto();
            dto.Title = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage(string.Format(
                      ErrorMessages.FieldIsRequired,
                      nameof(StreetcodeDTO.Title)));
        }

        [Fact]
        public void Should_Have_Error_When_DateString_Is_Empty()
        {
            var dto = CreateValidDto();
            dto.DateString = string.Empty;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.DateString)
                  .WithErrorMessage(string.Format(
                      ErrorMessages.FieldIsRequired,
                      nameof(StreetcodeDTO.DateString)));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_ViewCount_Is_Negative(int invalidCount)
        {
            var dto = CreateValidDto();
            dto.ViewCount = invalidCount;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.ViewCount)
                  .WithErrorMessage(string.Format(
                      ErrorMessages.ValueCannotBeNegative,
                      nameof(StreetcodeDTO.ViewCount)));
        }

        [Fact]
        public void Should_Have_Error_When_Event_End_Date_Is_Earlier_Than_Start_Date()
        {
            var dto = CreateValidDto();
            dto.EventStartOrPersonBirthDate = new DateTime(2020, 1, 1);
            dto.EventEndOrPersonDeathDate = new DateTime(2019, 1, 1);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EventEndOrPersonDeathDate)
                  .WithErrorMessage(ErrorMessages.EndDateCannotBeEarlierThanStartDate);
        }

        [Fact]
        public void Should_Have_Error_When_Teaser_Exceeds_Max_Length_Without_Paragraphs()
        {
            var dto = CreateValidDto();
            dto.Teaser = new string('A', 521);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Teaser)
                  .WithErrorMessage(string.Format(
                      ErrorMessages.TeaserLengthIsInvalid,
                      455,
                      520));
        }

        [Fact]
        public void Should_Have_Error_When_Tags_Is_Null()
        {
            var dto = CreateValidDto();
            dto.Tags = null!;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Tags)
                  .WithErrorMessage(string.Format(
                      ErrorMessages.CollectionIsRequired,
                      nameof(StreetcodeDTO.Tags)));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Negative(int invalidId)
        {
            var dto = CreateValidDto();
            dto.Id = invalidId;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage(string.Format(
                      ErrorMessages.ValueMustBeGreaterThanOrEqualTo,
                      nameof(StreetcodeDTO.Id),
                      0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Index_Is_Negative(int invalidIndex)
        {
            var dto = CreateValidDto();
            dto.Index = invalidIndex;

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Index)
                  .WithErrorMessage(string.Format(
                      ErrorMessages.ValueMustBeGreaterThanOrEqualTo,
                      nameof(StreetcodeDTO.Index),
                      0));
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = CreateValidDto();

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static StreetcodeDTO CreateValidDto()
        {
            return new StreetcodeDTO
            {
                Id = 1,
                Index = 1,
                Title = "Valid title",
                DateString = "1910",
                Alias = "valid-alias",
                TransliterationUrl = "valid-url",
                Status = StreetcodeStatus.Published,
                EventStartOrPersonBirthDate = new DateTime(1900, 1, 1),
                EventEndOrPersonDeathDate = new DateTime(1950, 1, 1),
                ViewCount = 0,
                Tags = new List<StreetcodeTagDTO>(),
                Teaser = "Valid teaser",
                StreetcodeType = StreetcodeType.Person
            };
        }
    }
}
