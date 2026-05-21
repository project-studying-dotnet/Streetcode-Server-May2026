using System;
using AutoMapper;
using FluentResults;
using MediatR;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;

public class GetCategoriesByStreetcodeIdTests
{
	public GetCategoriesByStreetcodeIdTests()
	{
	}
}
