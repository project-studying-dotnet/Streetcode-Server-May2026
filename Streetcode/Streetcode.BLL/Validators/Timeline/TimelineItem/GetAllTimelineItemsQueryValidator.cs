using FluentValidation;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem;

public sealed class GetAllTimelineItemsQueryValidator : AbstractValidator<GetAllTimelineItemsQuery>;