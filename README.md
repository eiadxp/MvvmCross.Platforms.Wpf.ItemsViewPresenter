# MvvmCross.Platforms.Wpf.ItemsViewPresenter
A simple library for MvvmCross that will enable the using of ItemsControls to host your views.... This include the Tabbed GUI for example.

# Terms:
## Container:
It is the an ItemsControl that will host your views. You can have multiple containers, but each one should be registerd in a unique name using attached property `MvxContainer.Id` in your container. If you registered more than one container with the same Id, the last one will be used. You should specify your container id of the views in the attribute `MvxWpfPresenter` in the code behind file.
## Holder:
It is a ContentControl that will host your view inside the container. If your container is TabControl, the holder will be a TabItem, otherwise it will be a ContentControl. You can change this by using the attached property `mvx:MvxContainer.ViewHolder` on the container, you can set this property to the your holder type.
## Holder Header:
You can set your holder header by using the attached property `MvxContainer.Header` in the root of your view. If your holder supports headers (`HeaderedContentControl`) the presenter will bind the attached property `MvxContainer.Header` to the holder header.
## View position:
You can set the way your view will be displayed in the attribute `MvxWpfPresenter` in the code behind file. You can choose one of the following values:
### New
Your view will be displayed in a new holder always.
### Active
Your view will be displayed in the active holder within the container, if the container is nor a `Selector` the view will be displayed in the last holder, and if you do not have any holders inside your container, the view will be displayed in a new holder.
### NewOrExsist
### NewOrHistoryExsist
