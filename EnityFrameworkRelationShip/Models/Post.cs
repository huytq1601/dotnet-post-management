using System.ComponentModel.DataAnnotations;

namespace EnityFrameworkRelationShip.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime DatePublished { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
