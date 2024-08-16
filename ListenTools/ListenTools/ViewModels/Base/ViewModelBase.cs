using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ListenTools.ViewModels.Base;

public class ViewModelBase : ObservableRecipient
{
    public IMessenger MessengerPub => this.Messenger;
}