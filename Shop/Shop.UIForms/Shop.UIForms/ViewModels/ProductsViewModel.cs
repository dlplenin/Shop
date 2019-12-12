using Shop.Common.Models;
using Shop.Common.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Shop.UIForms.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private ApiService apiService;
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value); }
        }

        public ProductsViewModel()
        {
            apiService = new ApiService();
            LoadProducts();
        }

        private async void LoadProducts()
        {
            var response = await apiService.GetListAsync<Product>("https://shopiste.azurewebsites.net", "/api", "/Products");

            if (!response.IsSuccess)
            { 
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            var products = (List<Product>)response.Result;
            Products = new ObservableCollection<Product>(products);
        }
    }
}
