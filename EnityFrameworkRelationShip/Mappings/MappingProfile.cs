using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Dtos.Tag;
using EnityFrameworkRelationShip.Dtos.User;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Tag, TagDto>().ReverseMap();

            CreateMap<Post, PostWithTagsDto>()
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Name)));

            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
