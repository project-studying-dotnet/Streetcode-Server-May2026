using AutoMapper;
using Streetcode.BLL.DTO.Comments;
using Streetcode.DAL.Entities.Comments;

namespace Streetcode.BLL.Mapping.Comments;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CreateCommentDto, Comment>();
        CreateMap<UpdateCommentDto, Comment>();
        CreateMap<Comment, CommentDto>();
    }
}
