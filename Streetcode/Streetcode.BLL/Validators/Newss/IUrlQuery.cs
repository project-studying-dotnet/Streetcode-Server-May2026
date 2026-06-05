using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;

namespace Streetcode.BLL.Validators.Newss
{
    public interface IUrlQuery
    {
        string Url { get; }
    }
}