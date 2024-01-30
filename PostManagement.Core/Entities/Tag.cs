using System.ComponentModel.DataAnnotations;

namespace PostManagement.Core.Entities
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
