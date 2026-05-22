using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Streetcode;

public class StreetcodePagedAndSortedSpecification : BaseSpecification<StreetcodeContent>
{
    public StreetcodePagedAndSortedSpecification(
        int page,
        int amount,
        string? title,
        string? sort,
        string? filter)
    {
        ApplyTitle(title);
        ApplyFilter(filter);
        ApplySort(sort);
        Skip = (page - 1) * amount;
        Take = amount;
    }

    private void ApplyTitle(string? title)
    {
        if (title is null)
        {
            return;
        }

        Criteria = s =>
            s.Title!.ToLower().Contains(title.ToLower()) ||
            s.Index.ToString() == title;
    }

    private void ApplyFilter(string? filter)
    {
        if (filter is null)
        {
            return;
        }

        var parts = filter.Split(':');
        if (parts.Length < 2)
        {
            return;
        }

        var filterValue = parts[1];
        Criteria = s => filterValue.Contains(s.Status.ToString());
    }

    private void ApplySort(string? sort)
    {
        if (sort is null)
        {
            return;
        }

        var column = sort.TrimStart('-');
        IsDescending = sort.StartsWith("-");

        OrderBy = column switch
        {
            "Title" => s => s.Title!,
            "Index" => s => s.Index,
            "CreatedAt" => s => s.CreatedAt,
            "UpdatedAt" => s => s.UpdatedAt,
            "EventStartOrPersonBirthDate" => s => s.EventStartOrPersonBirthDate,
            _ => null!,
        };

        if (OrderBy is null)
        {
            IsDescending = false;
        }
    }
}
