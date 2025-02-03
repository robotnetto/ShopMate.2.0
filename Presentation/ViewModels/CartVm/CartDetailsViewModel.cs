using CommunityToolkit.Mvvm.ComponentModel;
using ShopMate._2._0.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Presentation.ViewModels.CartVm
{
    public partial class CartDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid id;

        [ObservableProperty]
        private string title;

        public ObservableCollection<Item> Items { get; set; }
        public CartDetailsViewModel(Cart cart)
        {
            Id = cart.Id;
            Title = cart.Title;
            Items = new ObservableCollection<Item>(cart.Items);
        }
    }
}
