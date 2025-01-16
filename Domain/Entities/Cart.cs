using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Entities
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
