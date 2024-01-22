using EnityFrameworkRelationShip.Dtos.Tag;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface ITagsService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> GetTagByIdAsync(Guid id);
    }
}
