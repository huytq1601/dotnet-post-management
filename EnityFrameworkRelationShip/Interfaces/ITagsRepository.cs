
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface ITagsRepository
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
    }
}
