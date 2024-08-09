using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopMate._2._0.Applications.Services;
using ShopMate._2._0.Domain.Entities;
using ShopMate._2._0.Domain.Interfaces;
using ShopMate._2._0.Infrastructure.Repositories;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShopMate._2._0.Presentation.ViewModels.CartVm
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly ShopCartService shopCartService;

        [ObservableProperty]
        public ObservableCollection<CartDetailsViewModel> shopCarts = new();

        public ICommand AddCartCommand { get; }
        public CartViewModel(ShopCartService shopCartService)
        {
            this.shopCartService = shopCartService ?? throw new ArgumentNullException(nameof(shopCartService));
            AddCartCommand = new AsyncRelayCommand(OnAddCart);
        }
        public CartViewModel(): this(new ShopCartService(new ShopCartRepository( new Infrastructure.Data.LocalDbService())))
        {
        }

        private async Task OnAddCart()
        {
            throw new NotImplementedException();
        }
    }
}
