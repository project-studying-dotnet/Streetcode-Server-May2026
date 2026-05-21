using AutoMapper;
using FluentResults;
using MediatR;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;

public class GetCategoryByIdTests
{
    public GetCategoryByIdTests() { }

}