using CommunityToolkit.Mvvm.ComponentModel;
using ShopMate._2._0.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMate._2._0.Presentation.ViewModels.RecipeVm
{
    public partial class RecipeDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid id;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private bool favorite;

        [ObservableProperty]
        private string? imageStream;

        
     

        public RecipeDetailsViewModel(Recipe recipe)
        {
            Id = recipe.Id;
            Title = recipe.Title;
            Description = recipe.Description;
            Favorite = recipe.Favorite;
            ImageStream = recipe.ImageStream;
        }
    }
}
