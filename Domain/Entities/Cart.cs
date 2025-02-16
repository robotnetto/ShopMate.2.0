using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Entities
{
    public class Cart : INotifyPropertyChanged
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        private double _progressing;
        public double Progressing
        {
            get => _progressing;
            set
            {
                if (_progressing != value)
                {
                    _progressing = value;
                    OnPropertyChanged(nameof(Progressing));
                }
            }
        }
        public List<Item>? Items { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}