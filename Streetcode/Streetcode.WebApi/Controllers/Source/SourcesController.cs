using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Create;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Source;

public sealed class SourcesController : BaseApiController
{
    [HttpGet("names")]
    public async Task<IActionResult> GetAllNames(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllCategoryNamesQuery(), cancellationToken)
        );
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetAllCategoriesQuery(), cancellationToken)
        );
    }

    [HttpGet("category/{id:int}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetCategoryByIdQuery(id), cancellationToken)
        );
    }

    [HttpGet("category-content/{categoryId:int}/{streetcodeId:int}")]
    public async Task<IActionResult> GetCategoryContentByStreetcodeId([FromRoute] int streetcodeId, [FromRoute] int categoryId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetCategoryContentByStreetcodeIdQuery(streetcodeId, categoryId), cancellationToken)
        );
    }

    [HttpGet("streetcode/{streetcodeId:int}")]
    public async Task<IActionResult> GetCategoriesByStreetcodeId([FromRoute] int streetcodeId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new GetCategoriesByStreetcodeIdQuery(streetcodeId), cancellationToken)
        );
    }

    [HttpPost("category")]
    public async Task<IActionResult> CreateCategory([FromBody] SourceLinkCategoryDto category, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateSourceLinkCategoryCommand(category), cancellationToken)
        );
    }

    [HttpPut("category")]
    public async Task<IActionResult> UpdateCategory([FromBody] SourceLinkCategoryDto category, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateSourceLinkCategoryCommand(category), cancellationToken)
        );
    }

    [HttpDelete("category/{id:int}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteSourceLinkCategoryCommand(id), cancellationToken)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryContentCreateDto categoryContent, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new CreateStreetcodeCategoryContentCommand(categoryContent), cancellationToken)
        );
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CategoryContentUpdateDto categoryContent, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new UpdateStreetcodeCategoryContentCommand(categoryContent), cancellationToken)
        );
    }

    [HttpDelete("{streetcodeId:int}/{categoryId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int streetcodeId, [FromRoute] int categoryId, CancellationToken cancellationToken = default)
    {
        return base.HandleResult(
            await base.Mediator.Send(new DeleteStreetcodeCategoryContentCommand(streetcodeId, categoryId), cancellationToken)
        );
    }
}