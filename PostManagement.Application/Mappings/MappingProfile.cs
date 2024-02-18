using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PostManagement.Application.Dtos.Post;
using PostManagement.Application.Dtos.Role;
using PostManagement.Application.Dtos.Tag;
using PostManagement.Application.Dtos.User;
using PostManagement.Core.Entities;

namespace PostManagement.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Tag, TagDto>().ReverseMap();

            CreateMap<Post, PostWithTagsDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Name)))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => new SimpleUserDto
                {
                     Id = src.UserId,
                     FullName = $"{src.User.FirstName} {src.User.LastName}",
                     UserName = src.User.UserName!
                }));

            CreateMap<ApplicationUser, UserDto>();

            CreateMap<ApplicationUser, UserWithPermissionsDto>();

            CreateMap<IdentityRole, RoleDto>();

        }
    }
}
