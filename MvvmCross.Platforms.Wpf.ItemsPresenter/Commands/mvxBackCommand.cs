using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Commands
{
    public class mvxBackCommand : MvvmCross.Commands.IMvxCommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute()
        {
            return false;
        }
        public bool CanExecute(object parameter)
        {
            try
            {
                var holder = GetHolder(parameter);
                if (holder == null) return true;
                return !MvxContainer.GetHasClosingAction(holder);
            }
            catch (Exception)
            {
                
            }
            return true;
        }
        public void Execute()
        {
            throw new InvalidOperationException("A container must be passed in the parameter.");
        }
        public void Execute(object parameter)
        {
            var holder = GetHolder(parameter);
            if (holder == null) return;
            var view = holder.Content as FrameworkElement;
            if (view == null) return;
            object vm = null;
            if (view is MvvmCross.Views.IMvxView mvxv) vm = mvxv.ViewModel;
            if (vm == null) vm = view.DataContext;
            if (vm == null) return;
            MvxContainer.SetHasClosingAction(holder, true);
            RaiseCanExecuteChanged();
            var nav = Mvx.Resolve<Navigation.IMvxNavigationService>();
            nav.Close(vm as ViewModels.IMvxViewModel);
            MvxContainer.SetHasClosingAction(holder, false);
            RaiseCanExecuteChanged();
        }
        ContentControl GetHolder(object parameter)
        {
            var container = parameter as ItemsControl;
            if (container == null && parameter is string id)
                container = MvxContainer.GetContainerById(id);
            if (container == null)
                throw new InvalidOperationException("Can not find container");
            if (container.Items.Count == 0) return null;
            if (container is Selector selector)
                return selector.SelectedItem as ContentControl;
            return container.Items[container.Items.Count - 1] as ContentControl;
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
