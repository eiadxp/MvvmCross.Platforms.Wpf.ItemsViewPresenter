using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.ViewModels;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Core.ViewModels
{
    public class UserViewModel : MvxViewModel<int>
    {
        private int _Id;
        public int Id
        {
            get => _Id;
            set
            {
                if (SetProperty(ref _Id, value)) RaisePropertyChanged(nameof(Name));
            }
        }
        public string Name
        {
            get { return "User " + Id.ToString(); }
        }

        
        public override void Prepare(int parameter)
        {
            _Id = parameter;
        }

        public override string ToString() => Name;
    }
}
