using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;

namespace Streetcode.WebApi.Controllers.Source;

public class SourcesController : BaseApiController
{
    [HttpGet("names")]
    public async Task<IActionResult> GetAllNames()
    {
        return HandleResult(await Mediator.Send(new GetAllCategoryNamesQuery()));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories()
    {
        return HandleResult(await Mediator.Send(new GetAllCategoriesQuery()));
    }

    [HttpGet("category/{id:int}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetCategoryByIdQuery(id)));
    }

    [HttpGet("category-content/{categoryId:int}/{streetcodeId:int}")]
    public async Task<IActionResult> GetCategoryContentByStreetcodeId(
        [FromRoute] int streetcodeId,
        [FromRoute] int categoryId)
    {
        return HandleResult(await Mediator.Send(
            new GetCategoryContentByStreetcodeIdQuery(streetcodeId, categoryId)));
    }

    [HttpGet("streetcode/{streetcodeId:int}")]
    public async Task<IActionResult> GetCategoriesByStreetcodeId(
        [FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(
            new GetCategoriesByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost("category")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] SourceLinkCategoryDTO category)
    {
        return HandleResult(await Mediator.Send(
            new CreateSourceLinkCategoryCommand(category)));
    }

    [HttpPut("category")]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] SourceLinkCategoryDTO category)
    {
        return HandleResult(await Mediator.Send(
            new UpdateSourceLinkCategoryCommand(category)));
    }

    [HttpDelete("category/{id:int}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(
            new DeleteSourceLinkCategoryCommand(id)));
    }
}   