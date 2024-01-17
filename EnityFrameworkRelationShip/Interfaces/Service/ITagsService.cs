using EnityFrameworkRelationShip.Dtos.Tag;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Interfaces.Service
{
    public interface ITagsService
    {
        Task<IEnumerable<TagDto>> GetAllAsync();
    }
}
