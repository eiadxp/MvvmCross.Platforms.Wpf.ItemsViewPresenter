using MvvmCross.Platforms.Wpf.ItemsPresenter;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Demo.Views
{
    [MvxWpfPresenter("users",mvxViewPosition.NewOrExsist)]
    public partial class UserView
    {
        public UserView()
        {
            InitializeComponent();
        }
    }
}
