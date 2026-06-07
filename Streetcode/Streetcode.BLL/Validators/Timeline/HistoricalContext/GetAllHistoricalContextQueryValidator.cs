using FluentValidation;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;

namespace Streetcode.BLL.Validators.Timeline.HistoricalContext;

public sealed class GetAllHistoricalContextQueryValidator : AbstractValidator<GetAllHistoricalContextQuery>;