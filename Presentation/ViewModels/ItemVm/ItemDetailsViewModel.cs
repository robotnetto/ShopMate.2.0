using CommunityToolkit.Mvvm.ComponentModel;
using ShopMate._2._0.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Presentation.ViewModels.ItemVm
{
    public partial class ItemDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid _id;

        [ObservableProperty]
        private string _itemName;
        [ObservableProperty]
        private bool _isChecked;
        [ObservableProperty]
        private Guid _cartId;
        public ItemDetailsViewModel(Item item)
        {
            Id = item.Id;
            ItemName = item.ItemName;
            IsChecked = item.IsChecked;
            CartId = item.CartId;

        }
    }
}
