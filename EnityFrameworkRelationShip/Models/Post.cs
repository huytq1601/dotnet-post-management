﻿using System.ComponentModel.DataAnnotations;

namespace EnityFrameworkRelationShip.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = String.Empty;

        [Required]
        public string Content { get; set; } = String.Empty;

        public DateTime DatePublished { get; set; }

        // Navigation property for the many-to-many relationship
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}