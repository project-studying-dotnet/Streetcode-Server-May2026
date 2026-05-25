using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Term.Create;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public class TermController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTermsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTermByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTermDto term)
    {
        return HandleResult(await Mediator.Send(new CreateTermCommand(term)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTermCommand(id)));
    }
}
