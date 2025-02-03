using CommunityToolkit.Mvvm.ComponentModel;
using ShopMate._2._0.Presentation.ViewModels.CartVm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Presentation.ViewModels.ItemVm
{
    public  class ItemViewModel
    {
        private readonly CartDetailsViewModel selectedCart;

        public ItemViewModel(CartViewModel selectedCart)
        {
            this.selectedCart = selectedCart.SelectedCart;
        }
        



    }
}
