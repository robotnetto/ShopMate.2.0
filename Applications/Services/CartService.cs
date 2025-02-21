using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Exceptions;
using ShopMate._2._0.Domain.Interfaces;

namespace ShopMate._2._0.Applications.Services
{
    public class CartService
    {
        private readonly ICartRepository shopCartRepository;

        public CartService(ICartRepository shopCartRepository)
        {
            this.shopCartRepository = shopCartRepository;
        }

        public async Task AddNewCartAsync(Cart shopCart)
        {

            if (shopCart is null)
            {
                throw new ArgumentNullException(nameof(shopCart), "The shopcart parameter cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(shopCart.Title))
            {
                throw new ArgumentException("Title cannot be empty or whitespace!", nameof(shopCart.Title));
            }

            shopCart.Id = Guid.NewGuid();
            await shopCartRepository.CreateAsync(shopCart);
        }

        public async Task UpdateCartAsync(Cart shopCart)
        {
            if (string.IsNullOrWhiteSpace(shopCart.Title))
            {
                throw new ArgumentException("Title cannot be empty or whitespace!", nameof(shopCart.Title));
            }

            await shopCartRepository.UpdateAsync(shopCart);

        }

        public async Task<Cart> GetCartIdAsync(Guid id)
        {
            var result = await shopCartRepository.GetByIdAsync(id);

            return result ?? throw new NotFoundException("Cart not found!");

        }
        public async Task<IEnumerable<Cart>> GetAllCartsServiceAsync()
        {
            return await shopCartRepository.GetAllasync();

        }

        public async Task DeleteCartAsync(Cart shopCart)
        {
            await shopCartRepository.DeleteAsync(shopCart);
        }
    }
}
