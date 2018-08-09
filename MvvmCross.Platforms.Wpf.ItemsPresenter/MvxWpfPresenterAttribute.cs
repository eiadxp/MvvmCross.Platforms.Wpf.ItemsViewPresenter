using System;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using System.Windows;
using System.Linq;
using MvvmCross.ViewModels;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MvxWpfPresenterAttribute : MvxContentPresentationAttribute
    {
        public mvxViewPosition ViewPosition { get; set; }
        public string ContainerId { get; set; }
        public Func<object, string> ViewId { get; set; }
        static string DefaultViewId(object view) => view?.ToString();

        public MvxWpfPresenterAttribute(string containerId) : this(containerId, mvxViewPosition.New) { }
        public MvxWpfPresenterAttribute(string containerId, mvxViewPosition viewPosition)
        {
            ContainerId = containerId;
            ViewPosition = viewPosition;
            ViewId = DefaultViewId;
        }
        public MvxWpfPresenterAttribute(string containerId, mvxViewPosition viewPosition, string viewId) : this(containerId, viewPosition)
        {
            ViewId = (a) => viewId;
        }
        public MvxWpfPresenterAttribute(string containerId, mvxViewPosition viewPosition, Func<object, string> viewId) : this(containerId, viewPosition)
        {
            ViewId = viewId;
        }

        public string GetViewId(FrameworkElement view)
        {
            if (view is MvvmCross.Views.IMvxView mvxView)
                return ViewId(mvxView?.ViewModel ?? mvxView?.DataContext ?? view?.DataContext);
            return ViewId(view?.DataContext);
        }
        public static MvxWpfPresenterAttribute GetAttribute(FrameworkElement view, MvxViewModelRequest request)
        {
            if(view is MvvmCross.Presenters.IMvxOverridePresentationAttribute mvxView)
                if (mvxView.PresentationAttribute(request) is MvxWpfPresenterAttribute attr) return attr;
            return view.GetType().GetCustomAttributes(typeof(MvxWpfPresenterAttribute), true).FirstOrDefault() as MvxWpfPresenterAttribute;
        }
        public static string GetViewId(FrameworkElement view, MvxViewModelRequest request)
        {
            return GetAttribute(view, request)?.GetViewId(view);
        }
    }
}
