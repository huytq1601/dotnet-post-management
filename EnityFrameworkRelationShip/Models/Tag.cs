﻿using System.ComponentModel.DataAnnotations;

namespace EnityFrameworkRelationShip.Models
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
