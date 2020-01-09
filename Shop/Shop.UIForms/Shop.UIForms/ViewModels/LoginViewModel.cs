using GalaSoft.MvvmLight.Command;
using Shop.Common.Models;
using Shop.Common.Services;
using Shop.UIForms.Views;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Shop.UIForms.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private bool isRunning;
        private bool isEnabled;
        private readonly ApiService apiService;

        public bool IsRunning
        {
            get => this.isRunning;
            set => this.SetValue(ref this.isRunning, value);
        }

        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetValue(ref this.isEnabled, value);
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand => new RelayCommand(Login);

        public LoginViewModel()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
            Email = "dp@gmail.com";
            Password = "123456";
        }
        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You must enter an email", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You must enter a password", "Accept");
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var request = new TokenRequest
            {
                Password = this.Password,
                Username = this.Email
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var response = await this.apiService.GetTokenAsync(
                url,
                "/Account",
                "/CreateToken",
                request);

            this.IsRunning = false;
            this.IsEnabled = true;

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Email or password incorrect.", "Accept");
                return;
            }

            var token = (TokenResponse)response.Result;
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Token = token;
            mainViewModel.Products = new ProductsViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new ProductsPage());


            //if (!this.Email.Equals("dp@gmail.com") || !this.Password.Equals("123456"))
            //{
            //    await Application.Current.MainPage.DisplayAlert("Error", "Incorrect user or password", "Accept");
            //    return;
            //}

            //await Application.Current.MainPage.DisplayAlert("Ok", "Fuck yeah!!!", "Accept");

            //MainViewModel.GetInstance().Products = new ProductsViewModel();
            //await Application.Current.MainPage.Navigation.PushAsync(new ProductsPage());
        }
    }
}
