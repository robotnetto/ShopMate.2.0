using ShopMate._2._0.Domain.Entities;

namespace ShopMate._2._0.Domain.Interfaces
{
    public interface ICartRepository
    {

        Task CreateAsync(Cart shopCart);
        Task<IEnumerable<Cart>> GetAllasync();
        Task<Cart> GetByIdAsync(Guid id);
        Task UpdateAsync(Cart shopCart);
        Task DeleteAsync(Cart shopCart);
        Task AddNewItem(Item item);

    }
}
