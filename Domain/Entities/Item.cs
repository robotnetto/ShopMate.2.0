using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Entities
{
    public class Item : INotifyPropertyChanged
    {
        [Key]
        public Guid Id { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public bool _isChecked;
        public bool IsChecked
        {
           
            get => _isChecked;
            set
            {
                try
                {
                    if (_isChecked != value)
                    {
                        _isChecked = value;
                        OnPropertyChanged(nameof(IsChecked));
                    }
                }
                catch (Exception e)
                {

                    throw;
                }
               
            }
        }


        [ForeignKey("CartId")]
        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

