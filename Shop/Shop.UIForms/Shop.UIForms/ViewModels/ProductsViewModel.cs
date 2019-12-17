using Shop.Common.Models;
using Shop.Common.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace Shop.UIForms.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private readonly ApiService apiService;
        private ObservableCollection<Product> products;
        private bool isRefreshing;
        public ObservableCollection<Product> Products
        {
            get => this.products;
            set => this.SetValue(ref this.products, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetValue(ref this.isRefreshing, value);
        }

        public ProductsViewModel()
        {
            apiService = new ApiService();
            LoadProducts();
        }

        private async void LoadProducts()
        {
            IsRefreshing = true;
            var response = await apiService.GetListAsync<Product>("https://shopiste.azurewebsites.net", "/api", "/Products");
            

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                IsRefreshing = false;
                return;
            }

            var products = (List<Product>)response.Result;
            Products = new ObservableCollection<Product>(products);
            IsRefreshing = false;
        }
    }
}
