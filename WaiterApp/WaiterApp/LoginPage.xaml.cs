﻿using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Infrastructure.Repositories;
using Infrastructure.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WaiterApp
{
    public partial class LoginPage : ContentPage
    {
        private readonly ParametersLoader _parametersLoader;
        private readonly LoginViewModel _model;
        private bool _connectedToDb = false;
        public LoginPage(ParametersLoader parametersLoader)
        {
            InitializeComponent();
            _parametersLoader = parametersLoader;
            Task.Run(async () =>
            {
                await TestConnection();
            });

            _model = new LoginViewModel(parametersLoader);
            BindingContext = _model;

            if(bool.Parse(_parametersLoader.Parameters["remember"]))
            {
                _model.Username = _parametersLoader.Parameters["username"];
                Login(_parametersLoader.Parameters["password"]);
            }
        }

        protected async override void OnAppearing()
        {
            await TestConnection();
        }

        private async Task TestConnection()
        {
            var dbConnectionChecker = new DatabaseConnectionChecker();
            try
            {
                dbConnectionChecker.TestConnection();
                _connectedToDb = true;
            }
            catch (ConnectionStringException)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Db error", "Bad connection string. Configure before proceeding", "OK");
                });

                var settingsViewModel = new SettingsViewModel(_parametersLoader, false);
                await Navigation.PushAsync(new SettingsPage(settingsViewModel));
                _connectedToDb = false;
            }
        }

        private void OnSettingsButtonClick(object sender, EventArgs e)
        {
            var settingsViewModel = new SettingsViewModel(_parametersLoader, true);
            Navigation.PushAsync(new SettingsPage(settingsViewModel));
        }

        private void OnLoginButtonClick(object sender, EventArgs e)
        {
            if (_model.RememberUser)
            {
                _parametersLoader.SetParameter("remember", "true");
                _parametersLoader.SetParameter("username", _model.Username);
                _parametersLoader.SetParameter("password", PasswordEntry.Text);
            }
            else
            {
                _parametersLoader.SetParameter("remember", "false");
                _parametersLoader.SetParameter("username", string.Empty);
                _parametersLoader.SetParameter("password", string.Empty);
            }
            Login(PasswordEntry.Text);
        }

        private async void Login(string password)
        {
            if(!_connectedToDb)
            {
                return;
            }

            var waiter = await _model.LoginAsync(password);
            if (waiter != null)
            {
                _parametersLoader.SetParameter("waiterId", waiter.Id.ToString());
                _parametersLoader.SaveParameters();
                var page = new MainPage(new MainPageViewModel(
                    new OrderProductRepository(), new GroupRepository(), new SubgroupRepository(), new ProductRepository(),
                    new TableRepository(), new OrderRepository()));
                await Navigation.PushAsync(page);
            }
            else
            {
                await DisplayAlert("Login error", "Wrong credentials, please retry", "OK");
            }
        }

        protected override void OnDisappearing()
        {
            if (!_model.RememberUser)
            {
                _parametersLoader.SetParameter("remember", _model.RememberUser.ToString());
                _parametersLoader.SaveParameters();
            } 
        }
    }
}
