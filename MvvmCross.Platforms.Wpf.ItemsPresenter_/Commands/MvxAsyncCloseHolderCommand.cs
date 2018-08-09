using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Commands
{
    public class MvxAsyncCloseHolderCommand : MvxAsyncCommand
    {
        public MvxAsyncCloseHolderCommand()
        {
            base(ExecuteInternal);
        }
        protected override bool CanExecuteImpl(object parameter)
        {
            var holder = parameter as ContentControl;
            if (holder == null) return true;
            return !MvxContainer.GetHasClosingAction(holder);
        }
        protected override Task ExecuteAsyncImpl(object parameter)
        {
            return Task.Run(() => ExecuteInternal(parameter));
        }

        static void ExecuteInternal(object parameter)
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
                    nav.Close(mv);
                }
                MvxContainer.SetHasClosingAction(holder, false);
                RaiseCanExecuteChanged();
            }
        }
    }
}
