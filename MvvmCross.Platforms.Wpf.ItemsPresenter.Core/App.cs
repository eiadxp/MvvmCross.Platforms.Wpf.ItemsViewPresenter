using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.ViewModels;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<ViewModels.LoginViewModel>();
            base.Initialize();
        }
    }
}
