# MvvmCross.Platforms.Wpf.ItemsViewPresenter
A simple library for MvvmCross that will enable the using of ItemsControls to host your views.... This include the Tabbed GUI for example.

# Terms:
## Container:
It is the an ItemsControl that will host your views. You can have multiple containers, but each one should be registerd in a unique name using attached property `MvxContainer.Id` in your container. If you registered more than one container with the same Id, the last one will be used. You should specify your container id of the views in the attribute `MvxWpfPresenterAttribute` in the code behind file.
## Holder:
It is a ContentControl that will host your view inside the container. If your container is TabControl, the holder will be a TabItem, otherwise it will be a ContentControl. You can change this by using the attached property `mvx:MvxContainer.ViewHolder` on the container, you can set this property to the your holder type.
## Holder Header:
You can set your holder header by using the attached property `MvxContainer.Header` in the root of your view. If your holder supports headers (`HeaderedContentControl`) the presenter will bind the attached property `MvxContainer.Header` to the holder header.
## View ID:
Each view will have a string Id to enable the presenter from searching the views. The Id is calculated in the `MvxWpfPresenterAttribute.ViewId` function which will take a ViewModel as parameter and returns a string Id. the default impelmantation of this method is to return the view model `ToString()` function. This means that if did not override this method in your view model, the presenter will return any view that has the same view model type as your view model. you can change this be eather overrides the `ToString()` methods in your view model, or by providing a `ViewId` function in your view attribute `MvxWpfPresenterAttribute`.
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
