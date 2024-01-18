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
                .ForMember(dest => dest.TagNames,
                    opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name)));

            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
