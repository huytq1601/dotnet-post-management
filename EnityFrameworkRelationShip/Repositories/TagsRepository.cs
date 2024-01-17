using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Interfaces.Repository;
using EnityFrameworkRelationShip.Models;
using Microsoft.EntityFrameworkCore;

namespace EnityFrameworkRelationShip.Repositories
{
    public class TagsRepository: ITagsRepository
    {
        private readonly DataContext _context;

        public TagsRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags
                .Where(t => !t.IsDeleted)
                .ToListAsync();
        }
    }
}
