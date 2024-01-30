
using PostManagement.Application.Dtos.Tag;

namespace PostManagement.Application.Interfaces
{
    public interface ITagsService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> GetTagByIdAsync(Guid id);
    }
}
