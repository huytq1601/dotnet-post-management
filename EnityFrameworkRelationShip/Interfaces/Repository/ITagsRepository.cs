using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Interfaces.Repository
{
    public interface ITagsRepository
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
    }
}
