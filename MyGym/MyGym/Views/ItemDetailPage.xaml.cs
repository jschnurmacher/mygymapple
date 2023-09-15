using System.ComponentModel;
using Xamarin.Forms;
using MyGym.ViewModels;

namespace MyGym.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
