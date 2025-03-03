using Microsoft.EntityFrameworkCore;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {

        private readonly LocalDbService localDbService;

        public CartRepository(LocalDbService localDbService)
        {
            this.localDbService = localDbService;
        }

        public async Task CreateAsync(Cart shopCart)
        {

            await localDbService.ShopCarts.AddAsync(shopCart);
            await localDbService.SaveChangesAsync();

        }

        public async Task DeleteAsync(Cart shopCart)
        {
            localDbService.ShopCarts.Remove(shopCart);
            await localDbService.SaveChangesAsync();

        }

        public async Task<IEnumerable<Cart>> GetAllasync()
        {
            var result = await localDbService.ShopCarts.OrderBy( i => i.Title).Include(i => i.Items).ToListAsync();
            return result;
        }
        public async Task<Cart> GetByIdAsync(Guid id)
        {
            var result = await localDbService.ShopCarts.Where(s => s.Id == id).Include(i => i.Items).Include(p => p.Profile).FirstOrDefaultAsync();
            return result;
        }

        public async Task UpdateAsync(Cart shopCart)
        {
            localDbService.ShopCarts.Update(shopCart);
            await localDbService.SaveChangesAsync();
        }
        public async Task AddNewItem(Item item)
        {
            await localDbService.Items.AddAsync(item);
            await localDbService.SaveChangesAsync();
        }
    }
}
