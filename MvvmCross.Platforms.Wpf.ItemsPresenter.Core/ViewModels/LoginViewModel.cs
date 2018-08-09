using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.ViewModels;
using MvvmCross.Commands;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Core.ViewModels
{
    public class LoginViewModel : MvxViewModel
    {
        public LoginViewModel()
        {
            OkCommand = new MvxCommand(() => NavigationService.Navigate<HomeViewModel>());
        }
        private string _Name = "Admin";
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        private string _Password = "1234";
        public string Password
        {
            get { return _Password; }
            set { SetProperty(ref _Password, value); }
        }

        public IMvxCommand OkCommand { get; private set; }
    }
}
