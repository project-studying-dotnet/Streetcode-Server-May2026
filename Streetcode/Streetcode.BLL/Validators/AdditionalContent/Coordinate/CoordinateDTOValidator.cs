using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate
{
    public class CoordinateDtoValidator : AbstractValidator<CoordinateDTO>
    {
        private static readonly decimal MinLatitude = -90;
        private static readonly decimal MaxLatitude = 90;
        private static readonly decimal MinLongitude = -180;
        private static readonly decimal MaxLongitude = 180;
        public CoordinateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.Latitude)
                .InclusiveBetween(MinLatitude, MaxLatitude)
                .WithMessage(ErrorMessages.LatitudeMustBeBetweenMinus90And90);

            RuleFor(c => c.Longtitude)
                .InclusiveBetween(MinLongitude, MaxLongitude)
                .WithMessage(ErrorMessages.LongitudeMustBeBetweenMinus180And180);
        }
    }
}
