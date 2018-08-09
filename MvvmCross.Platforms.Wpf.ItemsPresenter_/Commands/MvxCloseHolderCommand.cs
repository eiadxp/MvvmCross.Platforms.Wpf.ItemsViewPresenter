using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Presenters;
using MvvmCross.Navigation;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Commands
{
    public class MvxCloseHolderCommand : MvvmCross.Commands.IMvxCommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute()
        {
            return false;
        }

        public bool CanExecute(object parameter)
        {
            var holder = parameter as ContentControl;
            if (holder == null) return true;
            return !MvxContainer.GetHasClosingAction(holder);
        }

        public void Execute()
        {
            throw new InvalidOperationException("You should pass the holder in command parameter.");
        }

        public async void Execute(object parameter)
        {
            var holder = parameter as ContentControl;
            if (holder == null) throw new InvalidCastException("Command parameter should be a content control.");
            var history = MvxContainer.GetHolderHistory(holder);
            if (history != null)
            {
                MvxContainer.SetHasClosingAction(holder, true);
                RaiseCanExecuteChanged();
                var nav = Mvx.Resolve<IMvxNavigationService>();
                foreach (var mv in history.Select((c) => MvxWpfPresenter.GetViewModel(c)).ToList())
                {
                    await nav.Close(mv);
                }
                MvxContainer.SetHasClosingAction(holder, false);
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
