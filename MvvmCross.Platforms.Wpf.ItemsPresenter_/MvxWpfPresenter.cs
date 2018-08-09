using MvvmCross.Logging;
using MvvmCross.Platforms.Wpf.Presenters;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.Presenters;
using MvvmCross.Presenters.Attributes;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter
{
    public class MvxWpfPresenter : MvxWpfViewPresenter
    {
        readonly ContentControl Root;

        public MvxWpfPresenter() : this(Application.Current?.MainWindow) { }
        public MvxWpfPresenter(ContentControl root) : base(root) => Root = root;

        /// <summary>
        /// First rgister <see cref="MvxWpfPresenterAttribute"/> in addition to the original MvvmCross attributes.
        /// </summary>
        public override void RegisterAttributeTypes()
        {
            AttributeTypesToActionsDictionary.Add(
                typeof(MvxWpfPresenterAttribute),
                new MvxPresentationAttributeAction()
                {
                    ShowAction = (viewType, attribute, request) => ShowView((MvxWpfPresenterAttribute)attribute, request),
                    CloseAction = (viewModel, attribute) => CloseViewModel(viewModel)
                });
            base.RegisterAttributeTypes();
        }
        /// <summary>
        /// May be not nessacery
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="viewType"></param>
        /// <returns></returns>
        public override MvxBasePresentationAttribute CreatePresentationAttribute(Type viewModelType, Type viewType)
        {
            var attr = viewType.GetCustomAttributes(typeof(MvxWpfPresenterAttribute), true);
            if ((attr?.Length).GetValueOrDefault() > 0) return attr.FirstOrDefault() as MvxWpfPresenterAttribute;
            return base.CreatePresentationAttribute(viewModelType, viewType);
        }

        private void ShowView(MvxWpfPresenterAttribute attribute, MvxViewModelRequest request)
        {
            //Try to find items container.
            ItemsControl container = string.IsNullOrWhiteSpace(attribute?.ContainerId) ?
                MvxContainer.GetFirstContainer() : MvxContainer.GetContainerById(attribute?.ContainerId);
            if (container == null)// If no container found the switch to content view.
            {
                ShowViewWithNoContainer(request, new MvxContentPresentationAttribute());
                return;
            }
            switch (attribute.ViewPosition)
            {
                case mvxViewPosition.Active:
                    ShowView_Active(attribute, request, container);
                    break;
                case mvxViewPosition.NewOrExsist:
                    ShowView_NewOrExist(attribute, request, container);
                    break;
                case mvxViewPosition.NewOrHistoryExsist:
                    ShowView_NewOrHistoryExist(attribute, request, container);
                    break;
                case mvxViewPosition.New:
                    ShowView_New(attribute, request, container);
                    break;
                default:
                    break;
            }
        }
        protected virtual void ShowViewWithNoContainer(MvxViewModelRequest request, MvxContentPresentationAttribute attribute)
        {
            var view = WpfViewLoader.CreateView(request);
            base.ShowContentView(view, new MvxContentPresentationAttribute(), request);
        }
        protected virtual void ShowView_Active(MvxWpfPresenterAttribute attribute, MvxViewModelRequest request, ItemsControl container)
        {
            var selector = container as Selector;
            int i = (selector == null) ? selector.Items.Count - 1 : selector.SelectedIndex;
            if (i < 0) i = container.Items.Count - 1;
            if (i > -1 && container.Items[i] is ContentControl holder)
            {
                var view = WpfViewLoader.CreateView(request);
                GetHistory(holder).Push(view);
                SetViewInHolder(holder, view);
            }
            else ShowView_New(attribute, request, container);
        }
        protected virtual void ShowView_NewOrExist(MvxWpfPresenterAttribute attribute, MvxViewModelRequest request, ItemsControl container)
        {
            ContentControl holder = null;
            var viewModel = (request as MvxViewModelInstanceRequest)?.ViewModelInstance;
            foreach (var item in container.Items.OfType<ContentControl>())
            {
                if (attribute?.ViewId(viewModel) == MvxWpfPresenterAttribute.GetViewId(item.Content as FrameworkElement, request))
                {
                    holder = item;
                    break;
                }
            }
            if (holder != null)
            {
                if (container is Selector selector) selector.SelectedItem = holder;
                else holder.BringIntoView();
            }
            else
            {
                ShowView_New(attribute, request, container);
            }
        }
        protected virtual void ShowView_NewOrHistoryExist(MvxWpfPresenterAttribute attribute, MvxViewModelRequest request, ItemsControl container)
        {
            ContentControl holder = null;
            var viewModel = (request as MvxViewModelInstanceRequest)?.ViewModelInstance;
            foreach (var item in container.Items.OfType<ContentControl>())
            {
                var history = MvxContainer.GetHolderHistory(item);
                foreach (var view in history)
                {
                    if (attribute?.ViewId(viewModel) == MvxWpfPresenterAttribute.GetViewId(view, request))
                    {
                        holder = item;
                        break;
                    }
                }
            }
            if (holder != null)
            {
                if (container is Selector selector) selector.SelectedItem = holder;
                else holder.BringIntoView();
            }
            else
            {
                ShowView_New(attribute, request, container);
            }
        }
        protected virtual void ShowView_New(MvxWpfPresenterAttribute attribute, MvxViewModelRequest request, ItemsControl container)
        {
            var view = WpfViewLoader.CreateView(request);
            var holder = CreateViewHolder(container);
            SetViewInHolder(holder, view);
            GetHistory(holder).Push(view);
            var i = container.Items.Add(holder);
            if (container is Selector selector) selector.SelectedIndex = i;
        }

        /// <summary>
        /// This will close the view containing a view model and return to previous view, 
        /// or close the holder if it is the only view in the navigation stack.
        /// </summary>
        /// <param name="viewModel">The view model to be closed.</param>
        /// <returns>True if a view is found and closed, otherwise it will return false.</returns>
        private bool CloseViewModel(IMvxViewModel viewModel)
        {
            bool result = false;
            var view = GetView(viewModel);
            if (view != null) result = CloseView(view);
            return result;
        }
        private bool CloseView(FrameworkElement view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            var holder = view.Parent as ContentControl;
            if (holder != null)
            {
                var container = holder.Parent as ItemsControl;
                if (container != null)
                {
                    var history = MvxContainer.GetHolderHistory(holder);
                    if (history == null || history.Count < 2) //The holder contains only one view or no history
                    {
                        history?.Clear();
                        MvxContainer.SetHolderHistory(holder, null);
                        holder.Content = null;
                        container.Items.Remove(holder);
                        return true;
                    }
                    else
                    {
                        if (history.Peek() != view)
                            throw new InvalidOperationException("You can only close last opened view.");
                        history.Pop();
                        SetViewInHolder(holder, history.Peek());
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Close(IMvxViewModel toClose)
        {
            System.Threading.Tasks.Task.Delay(2000).Wait();
            if (CloseViewModel(toClose)) return;
            base.Close(toClose);
        }

        ContentControl CreateViewHolder(ItemsControl container)
        {
            var t = MvxContainer.GetViewHolder(container);
            ContentControl holder;
            if (container is TabControl tabControl)
            {
                TabItem tabHolder = (t == null) ? new TabItem() : Activator.CreateInstance(t) as TabItem;
                if (tabHolder == null) tabHolder = new TabItem();
                holder = tabHolder;
            }
            else
            {
                if (t == null) holder = new ContentControl();
                else holder = Activator.CreateInstance(t) as ContentControl;
            }
            return holder;
        }
        void SetViewInHolder(ContentControl holder, FrameworkElement view)
        {
            holder.Content = view;
            if (holder is HeaderedContentControl headered)
            {
                var binding = new System.Windows.Data.Binding() { Source = view, Mode = BindingMode.TwoWay };
                binding.Path = new PropertyPath("(0)", MvxContainer.HeaderProperty);
                BindingOperations.SetBinding(headered, HeaderedContentControl.HeaderProperty, binding);
            }
        }
        Stack<FrameworkElement> GetHistory(ContentControl holder)
        {
            var history = MvxContainer.GetHolderHistory(holder);
            if (history == null)
            {
                history = new Stack<FrameworkElement>();
                MvxContainer.SetHolderHistory(holder, history);
            }
            return history;
        }
        FrameworkElement GetView(IMvxViewModel viewModel)
        {
            foreach (var container in MvxContainer.GetContainers())
            {
                foreach (var holder in container.Items.OfType<ContentControl>())
                {
                    var control = holder.Content as FrameworkElement;
                    if (control is IMvxWpfView view && Equals(viewModel, view.ViewModel)) return control;
                    if (Equals(control.DataContext, viewModel)) return holder;
                }
            }
            return null;
        }
        FrameworkElement GetViewInHistory(IMvxViewModel viewModel)
        {
            foreach (var container in MvxContainer.GetContainers())
            {
                foreach (var holder in container.Items.OfType<ContentControl>())
                {
                    foreach (var control in GetHistory(holder))
                    {
                        if (control is IMvxWpfView view && Equals(viewModel, view.ViewModel)) return control;
                        if (Equals(control.DataContext, viewModel)) return holder;
                    }
                }
            }
            return null;
        }
        static internal IMvxViewModel GetViewModel(FrameworkElement view)
        {
            if (view is IMvxWpfView mvx) return mvx.ViewModel;
            return view.DataContext as IMvxViewModel;
        }
    }
}
