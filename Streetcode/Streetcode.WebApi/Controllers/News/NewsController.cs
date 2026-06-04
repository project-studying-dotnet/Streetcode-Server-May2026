using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.Newss.Create;     
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.BLL.MediatR.Newss.GetAll;
using Streetcode.BLL.MediatR.Newss.GetById;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.Attributes;

namespace Streetcode.WebApi.Controllers
{
    public class NewsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllNewsQuery()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetNewsByIdQuery(id)));
        }

        [HttpGet("{url}")]
        public async Task<IActionResult> GetByUrl([FromRoute] string url)
        {
            return HandleResult(await Mediator.Send(new GetNewsByUrlQuery(url)));
        }

        [HttpGet("sorted")]
        public async Task<IActionResult> GetSortedByDateTime()
        {
            return HandleResult(await Mediator.Send(new SortedByDateTimeQuery()));
        }

        [HttpGet("with-links")]
        public async Task<IActionResult> GetNewsAndLinksByUrl([FromQuery] string url)
        {
            return HandleResult(await Mediator.Send(new GetNewsAndLinksByUrlQuery(url)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewsDTO newsDto)
        {
            return HandleResult(await Mediator.Send(new CreateNewsCommand(newsDto)));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] NewsDTO newsDto)
        {
            newsDto.Id = id;

            return HandleResult(await Mediator.Send(new UpdateNewsCommand(newsDto)));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteNewsCommand(id)));
        }
    }
}