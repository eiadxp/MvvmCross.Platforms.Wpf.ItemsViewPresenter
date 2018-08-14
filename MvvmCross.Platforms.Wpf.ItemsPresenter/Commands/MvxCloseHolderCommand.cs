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
    /// <summary>
    /// This class is a command to close a holder with all its views.
    /// </summary>
    /// <remarks>
    /// <para>You don not need to create this class, just use the static property <see cref="MvxWpfPresenter.CloseHolderCommand"/> instead,
    ///  the class behavior is controlled by the command parameter.</para>
    ///  <para>If the command parameter is a <see cref="ItemsControl"/> the command will get the selected holder
    ///   or the last holder in the items and close the last open view in it.</para>
    ///  <para>If the command parameter is a <see cref="string"/>, the class will search for a container with 
    ///  same id, and if it found it will perform the same previous procedure for it.</para>
    ///  <para>If the command parameter is a <see cref="ContentControl"/> the command will close all the views 
    ///  in the navigation stack of that holder and then remove the holder from the container.</para>
    /// </remarks>
    public class MvxCloseHolderCommand : MvvmCross.Commands.IMvxCommand
    {
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Returns false always, because we should specify a holder or container.
        /// </summary>
        /// <returns>false always.</returns>
        public bool CanExecute()
        {
            return false;
        }
        /// <summary>
        /// Returns true if parameter has a value that can lead to a holder.
        /// </summary>
        /// <param name="parameter">
        /// Could be a container id (<c>string</c>), container (<see cref="ItemsControl"/>), or holder (<see cref="ContentControl"/>).
        /// </param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            try
            {
                var holder = MvxCloseViewCommand.GetHolder(parameter);
                if (holder == null) return true;
                return !MvxContainer.GetHasClosingAction(holder);
            }
            catch (Exception)
            {

            }
            return true;
        }
        /// <summary>
        /// Can not be called because we should specify a holder or container.
        /// </summary>
        public void Execute()
        {
            throw new InvalidOperationException("You should pass the holder in command parameter.");
        }

        /// <summary>
        /// Execute a close command on a holder.
        /// </summary>
        /// <param name="parameter">
        /// Could be a container id (<c>string</c>), container (<see cref="ItemsControl"/>), or holder (<see cref="ContentControl"/>).
        /// </param>
        /// <remarks>
        /// If the parameter is <see cref="string"/>, the command will search for the container of same id
        /// and if the parameter is <see cref="ItemsControl"/> it will use it as a container.
        /// In both cases the command will get the selected holder in the container (if it is a <see cref="Selector"/>)
        /// or it will use the last holder in the items collection.
        /// The close command is will close all the views in the holder navigation stack.
        /// </remarks>
        public async void Execute(object parameter)
        {
            var holder = MvxCloseViewCommand.GetHolder(parameter);
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
