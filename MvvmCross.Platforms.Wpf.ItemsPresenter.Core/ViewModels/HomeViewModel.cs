using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.ViewModels;
using MvvmCross.Commands;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
    {
        public HomeViewModel()
        {
            FirstCommand = new MvxCommand(() => NavigationService.Navigate<FirstViewModel>());
            SecondCommand = new MvxCommand(() => NavigationService.Navigate<SecondViewModel>());
            ThirdCommand = new MvxCommand(() => NavigationService.Navigate<ThirdViewModel>());
            UserCommand = new MvxCommand<object>((i) => NavigationService.Navigate(new UserViewModel(), (int)i));
        }
        public IMvxCommand FirstCommand { get; set; }
        public IMvxCommand SecondCommand { get; set; }
        public IMvxCommand ThirdCommand { get; set; }
        public IMvxCommand<object> UserCommand { get; set; }

    }
}
