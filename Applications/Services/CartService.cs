using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Exceptions;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using ShopMate._2._0.Presentation.ViewModels.ProfileViewModel;

namespace ShopMate._2._0.Applications.Services
{
    public class CartService
    {
        private readonly ICartRepository cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        public async Task<Cart> AddNewCartAsync(string cartName, ProfileDetailsViewModel profileDetailsViewModel)
        {

            if (string.IsNullOrWhiteSpace(cartName))
            {
                throw new ArgumentException("Title cannot be empty or whitespace!");
            }
            Cart newCart = new() { Title = cartName, Items = new List<Item>(), ProfileId = profileDetailsViewModel.Id };

            newCart.Id = Guid.NewGuid();
            await cartRepository.CreateAsync(newCart);

            return newCart;
        }

        public async Task<Cart> UpdateCartAsync(Guid cartId, string cartName)
        {
            if(cartId == Guid.Empty)
            {
                throw new ArgumentNullException( "Cant find cart id!");
            }
            if (string.IsNullOrWhiteSpace(cartName))
            {
                throw new ArgumentException("Title cannot be empty or whitespace!");
            }
            var cart = await cartRepository.GetByIdAsync(cartId);
            if(cart != null)
            {
                cart.Title = cartName;
                await cartRepository.UpdateAsync(cart);
            }

            return cart!;

        }
        public async Task<Item> AddItemToCartAsync(Guid cartId, FoodData foodData)
        {
            if (cartId == Guid.Empty)
            {
                throw new ArgumentException("Cart ID cannot be empty!", nameof(cartId));
            }
            if (foodData == null)
            {
                throw new ArgumentException("Item cannot be null!", nameof(foodData));
            }

            var cart = await cartRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new NotFoundException("Cart not found!");
            }

            var newItem = new Item { ItemName = foodData.Name, IsChecked = false, CartId = cartId};
            cart.Items!.Add(newItem);
            await cartRepository.UpdateAsync(cart);

            return newItem;
        }

        public async Task UpdateItemCheckedStatusAsync(Guid cartId, Guid itemId, bool isChecked)
        {
            if (cartId == Guid.Empty)
                throw new ArgumentNullException(nameof(cartId), "Cart ID not found!");

            if (itemId == Guid.Empty)
                throw new ArgumentNullException(nameof(itemId), "Item ID not found!");

            var cart = await cartRepository.GetByIdAsync(cartId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found!");

            var item = cart.Items!.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found in cart!");

            item.IsChecked = isChecked;
            await cartRepository.UpdateAsync(cart);
        }

        public async Task DeleteItemFromCartAsync(Guid cartId, Guid itemId)
        {
            if (cartId == Guid.Empty)
                throw new ArgumentNullException(nameof(cartId), "Cart ID not found!");
            if (itemId == Guid.Empty)
                throw new ArgumentNullException(nameof(itemId), "Item ID not found!");
            var cart = await cartRepository.GetByIdAsync(cartId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found!");
            var item = cart.Items!.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found in cart!");
            cart.Items!.Remove(item);
            await cartRepository.UpdateAsync(cart);
        }

        public async Task<Cart> GetCartIdAsync(Guid id)
        {
            var result = await cartRepository.GetByIdAsync(id);

            return result ?? throw new NotFoundException("Cart not found!");

        }
        public async Task<IEnumerable<Cart>> GetAllCartsServiceAsync()
        {
            return await cartRepository.GetAllasync();

        }
        public async Task AddNewItem(Item item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item), "The item parameter cannot be null!");
            }
            await cartRepository.AddNewItem(item);
        }

        public async Task DeleteCartAsync(Cart shopCart)
        {
            await cartRepository.DeleteAsync(shopCart);
        }
    }
}
