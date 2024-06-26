using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Entities
{
    public class Recipe
    {
        [Key]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public bool Favorite { get; set; }
        public string? ImageStream { get; set; }
    }
}
