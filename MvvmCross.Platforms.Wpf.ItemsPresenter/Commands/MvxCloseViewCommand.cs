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
    /// <summary>
    /// This class is a command to close the last view in a holder.
    /// </summary>
    /// <remarks>
    /// <para>You don not need to create this class, just use the static property <see cref="MvxWpfPresenter.CloseViewCommand"/> instead,
    ///  the class behavior is controlled by the command parameter.</para>
    ///  <para>If the command parameter is a <see cref="ItemsControl"/> the command will get the selected holder
    ///   or the last holder in the items and close the last open view in it.</para>
    ///  <para>If the command parameter is a <see cref="string"/>, the class will search for a container with 
    ///  same id, and if it found it will perform the same previous procedure for it.</para>
    ///  <para>If the command parameter is a <see cref="ContentControl"/> the command will close the last 
    ///  view inside the navigation stack.</para>
    ///  <para>If there is no views remaining in the navigation stack, the holder will be removed from the
    ///  container.</para>
    /// </remarks>
    public class MvxCloseViewCommand : MvvmCross.Commands.IMvxCommand
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
                var holder = GetHolder(parameter);
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
            throw new InvalidOperationException("A container must be passed in the parameter.");
        }
        /// <summary>
        /// Execute a back command on a holder.
        /// </summary>
        /// <param name="parameter">
        /// Could be a container id (<c>string</c>), container (<see cref="ItemsControl"/>), or holder (<see cref="ContentControl"/>).
        /// </param>
        /// <remarks>
        /// If the parameter is <see cref="string"/>, the command will search for the container of same id
        /// and if the parameter is <see cref="ItemsControl"/> it will use it as a container.
        /// In both cases the command will get the selected holder in the container (if it is a <see cref="Selector"/>)
        /// or it will use the last holder in the items collection.
        /// The back command is actually a close command for the last view in the holder.
        /// </remarks>
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
        internal static ContentControl GetHolder(object parameter)
        {
            var container = parameter as ItemsControl;
            ContentControl holder = null;
            if (container == null && parameter is string id)
                container = MvxContainer.GetContainerById(id);
            if (container != null && container.Items.Count > 0)
            {
                if (container is Selector selector)
                    holder = selector.SelectedItem as ContentControl;
                holder = container.Items[container.Items.Count - 1] as ContentControl;
            }
            if (holder == null)
                holder = parameter as ContentControl;
            return holder;
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
