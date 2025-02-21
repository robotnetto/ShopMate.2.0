using CommunityToolkit.Mvvm.ComponentModel;
using ShopMate._2._0.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Presentation.ViewModels.CartVm
{
    public partial class CartDetailsViewModel : ObservableObject
    {
        private readonly Cart _cart;

        [ObservableProperty]
        private Guid _id;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        public double _progressing;
        public ObservableCollection<Item> Items { get; set; }
        public CartDetailsViewModel(Cart cart)
        {
            this._cart = cart;
            Id = cart.Id;
            Title = cart.Title;
            Progressing = cart.Progressing;
            Items = new ObservableCollection<Item>(cart.Items);

            _cart.PropertyChanged += Cart_PropertyChanged;

        }
    
    
        private void Cart_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Cart.Progressing))
            {
                Progressing = _cart.Progressing; 
            }
        }
    } 
}
