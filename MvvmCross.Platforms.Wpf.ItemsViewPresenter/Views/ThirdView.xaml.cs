using MvvmCross.Platforms.Wpf.ItemsPresenter;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Demo
{
    [MvxWpfPresenter("docs", mvxViewPosition.NewOrExsist)]
    public partial class ThirdView
    {
        public ThirdView()
        {
            InitializeComponent();
        }
    }
}
