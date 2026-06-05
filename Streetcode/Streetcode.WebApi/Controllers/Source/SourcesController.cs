using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Create;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;
using Streetcode.DAL.Enums;

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
        [FromBody] SourceLinkCategoryDto category)
    {
        return HandleResult(await Mediator.Send(
            new CreateSourceLinkCategoryCommand(category)));
    }

    [HttpPut("category")]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] SourceLinkCategoryDto category)
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

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CategoryContentCreateDto categoryContent)
    {
        return HandleResult(await Mediator.Send(
            new CreateStreetcodeCategoryContentCommand(categoryContent)));
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] CategoryContentUpdateDto categoryContent)
    {
        return HandleResult(await Mediator.Send(
            new UpdateStreetcodeCategoryContentCommand(categoryContent)));
    }

    [HttpDelete("{streetcodeId:int}/{categoryId:int}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int streetcodeId,
        [FromRoute] int categoryId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteStreetcodeCategoryContentCommand(streetcodeId, categoryId)));
    }
}