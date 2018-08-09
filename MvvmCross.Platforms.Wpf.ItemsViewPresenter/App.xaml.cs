using MvvmCross.Core;

namespace MvvmCross.Platforms.Wpf.ItemsPresenter.Demo
{
    public partial class App
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<MvxWpfSetup<Core.App>>();
        }
    }
}
