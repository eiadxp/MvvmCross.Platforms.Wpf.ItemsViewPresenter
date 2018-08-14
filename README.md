# MvvmCross.Platforms.Wpf.ItemsViewPresenter
A simple library for MvvmCross that will enable the using of ItemsControls to host your views.... This include the Tabbed GUI for example.

# Terms:
## Container:
It is the an ItemsControl that will host your views. You can have multiple containers, but each one should be registered in a unique name using attached property `MvxContainer.Id` in your container. If you registered more than one container with the same Id, the last one will be used. You should specify your container id of the views in the attribute `MvxWpfPresenterAttribute` in the code behind file.
## Holder:
It is a ContentControl that will host your view inside the container. If your container is TabControl, the holder will be a TabItem, otherwise it will be a ContentControl. You can change this by using the attached property `mvx:MvxContainer.HolderType` on the container, you can set this property to the your holder type.
## Holder Header:
You can set your holder header by using the attached property `MvxContainer.Header` in the root of your view. If your holder supports headers (`HeaderedContentControl`) the presenter will bind the attached property `MvxContainer.Header` to the holder header.
## View ID:
Each view will have a string Id to enable the presenter from searching the views. The Id is calculated in the `MvxWpfPresenterAttribute.ViewId` function which will take a ViewModel as parameter and returns a string Id. the default implementation of this method is to return the view model `ToString()` function. This means that if did not override this method in your view model, the presenter will return any view that has the same view model type as your view model. you can change this be either overrides the `ToString()` methods in your view model, or by providing a `ViewId` function in your view attribute `MvxWpfPresenterAttribute`.
## View position:
You can set the way your view will be displayed in the attribute `MvxWpfPresenterAttribute` in the code behind file. You can choose one of the following values:
 1. **New**:
 Your view will be displayed in a new holder always.
 2. **Active**:
 Your view will be displayed in the active holder within the container, if the container is nor a `Selector` the view will be displayed in the last holder, and if you do not have any holders inside your container, the view will be displayed in a new holder.
 3. **NewOrExsist**:
 The presenter will search for any matching visible view, and if it found one it will bring its holder to the top, otherwise it will display the view in a new holder. The view searching is done using ViewId function of the `MvxWpfPresenterAttribute`.
 4. **NewOrHistoryExsist**:
 The presenter will search for the matching view in every holder and in the navigation stack of every holder, if it found a matching view, its holder will be displayed, otherwise it will display the view in a new holder.
# Usage:
1. The easiest way to start is to use our setup class by registering it in the app class of your UI project:
```C#
public partial class App : MvxApplication
{
    protected override void RegisterSetup()
    {
        this.RegisterSetupType<MvxWpfSetup<Core.App>>();
    }
}
```
Or you can create your own setup class that returns `MvxWpfPresenter` for the `CreateViewPresenter` method, and register it as before:
```C#
public class MySetup : Core.MvxWpfSetup<App>
{
    protected override IMvxWpfViewPresenter CreateViewPresenter(ContentControl root)
    {
        return new MvxWpfPresenter(root);
    }
}
```
2. Our presenter is based on MvvmCross presenter `MvxWpfViewPresenter` which means that you can use the content and window views of the MvvmCross normally. When you need to use ItemsControl presentation, you should register an ItemsControl as a container using the attached property `MvxContainer.Id`. You can also use the attached property `MvxContainer.HolderType` to set the holder type of your view. In the following code we registered a TabControl with Id "docs" and we kept the default holder type (`TabItem`), we also registered a ListBox as a container with id "users" and set the holder type to `Expander`, so all the views inside this container will be placed in an `Expander` control:
```XAML
<view:MvxWpfView x:Class="MvvmCross.Platforms.Wpf.ItemsPresenter.Demo.Views.HomeView"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:view="clr-namespace:MvvmCross.Platforms.Wpf.Views;assembly=MvvmCross.Platforms.Wpf"
                 xmlns:mvx="clr-namespace:MvvmCross.Platforms.Wpf.ItemsPresenter;assembly=MvvmCross.Platforms.Wpf.ItemsPresenter">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="1*"/>
            <ColumnDefinition Width="2*"/>
        </Grid.ColumnDefinitions>
        <TabControl mvx:MvxContainer.Id="docs"/>
        <ListBox Grid.Column="1" mvx:MvxContainer.Id="users" mvx:MvxContainer.HolderType="{x:Type Expander}"/>
    </Grid>
</view:MvxWpfView>
```
3. Create your views as you normally do with MvvmCross.
4. You can use the attached property `MvxContainer.Header` in your view to set the holder header if your it supports headers (based on `HeaderedContentControl` like `TabItem` or `Expander`):
```XAML
<view:MvxWpfView x:Name="mvxWpfView" x:Class="MvvmCross.Platforms.Wpf.ItemsPresenter.Demo.Views.FirstView"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:view="clr-namespace:MvvmCross.Platforms.Wpf.Views;assembly=MvvmCross.Platforms.Wpf"
                 xmlns:mvx="clr-namespace:MvvmCross.Platforms.Wpf.ItemsPresenter;assembly=MvvmCross.Platforms.Wpf.ItemsPresenter"
                 mvx:MvxContainer.Header="First View">

</view:MvxWpfView>
```
Please note that the header of your holder is Binded to the attached property `MvxContainer.Header`, so whenever you change the attached property, the holder header will change also.

5. In the code behind file use the attribute `MvxWpfPresenter` and provide at least the container id. you can also set other parameters like ViewPosition and ViewId:
```C#
[MvvmCross.Platforms.Wpf.ItemsPresenter.MvxWpfPresenter("docs")]
public partial class FirstView
{
    public FirstView()
    {
        InitializeComponent();
    }
}
```

# Navigation stack:
When ever you show a view in an existed holder the new view will be pushed to the navigation stack in front of the old one. so when you close a view, the navigation stack will pop the last view out and bring the previous one to front, then it will this view.

# GUI Commands:
The `MvxWpfPresenter` contains two static commands to be used directly in your GUI:

1. `MvxWpfPresenter.CloseViewCommand`: This command will close last view in a holder, bring the previous view from the navigation stack, and may remove the holder from its container if there is no remaining views in the navigation stack.
2. `MvxWpfPresenter.CloseHolderCommand`: This command will close all the views in a holder, and remove the holder from its container.

Both commands use the command parameter to specify the target holder. The command parameter could be:
1. The holder to be used.
2. A container, and here the command will get the selected holder if available (Your container is a `Selector` class), or it will use the last holder in the container items.
3. A container id (`string`), and the command will get the container which is registered by that id and continue as descried above.
