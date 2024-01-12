﻿using System.ComponentModel.DataAnnotations;

namespace EnityFrameworkRelationShip.Models
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        // Navigation property for the many-to-many relationship
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}