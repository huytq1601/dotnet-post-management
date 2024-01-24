using EnityFrameworkRelationShip.Dtos.Tag;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface ITagsService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> GetTagByIdAsync(Guid id);
    }
}
