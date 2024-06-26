using ShopMate._2._0.Domain.Entities;

namespace ShopMate._2._0.Domain.Interfaces
{
    public interface IShopCartRepository
    {

        Task CreateAsync(ShopCart shopCart);
        Task<IEnumerable<ShopCart>> GetAllasync();
        Task<ShopCart> GetByIdAsync(Guid id);
        Task UpdateAsync(ShopCart shopCart);
        Task DeleteAsync(ShopCart shopCart);

    }
}
