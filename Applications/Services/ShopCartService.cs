using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Exceptions;
using ShopMate._2._0.Domain.Interfaces;

namespace ShopMate._2._0.Applications.Services
{
    public class ShopCartService
    {
        private readonly IShopCartRepository shopCartRepository;

        public ShopCartService(IShopCartRepository shopCartRepository)
        {
            this.shopCartRepository = shopCartRepository;
        }

        public async Task AddNewCart(ShopCart shopCart)
        {

            if (shopCart is null)
                throw new ArgumentNullException(nameof(shopCart), "The shopCart parameter cannot be null!");

            if (string.IsNullOrWhiteSpace(shopCart.Title))
                throw new ArgumentException("Title cannot be empty or whitespace!", nameof(shopCart.Title));

            shopCart.Id = Guid.NewGuid();
            await shopCartRepository.CreateAsync(shopCart);
        }

        public async Task UpdateCart(ShopCart shopCart)
        {
            if (string.IsNullOrWhiteSpace(shopCart.Title))
                throw new ArgumentException("Title cannot be empty or whitespace!", nameof(shopCart.Title));

            await shopCartRepository.UpdateAsync(shopCart);

        }

        public async Task<ShopCart> GetCartId(Guid id)
        {
            var result = await shopCartRepository.GetByIdAsync(id);

            return result ?? throw new NotFoundException("Cart not found!");

        }
        public async Task<IEnumerable<ShopCart>> GetAllCarts()
        {
            var result = await shopCartRepository.GetAllasync();

            return result ?? Enumerable.Empty<ShopCart>();
        }

        public async Task DeleteCart(ShopCart shopCart)
        {
            await shopCartRepository.DeleteAsync(shopCart);
        }
    }
}
