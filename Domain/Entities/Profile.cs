using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Entities
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        
        public List<Cart>? Carts { get; set; }
        
    }
}
