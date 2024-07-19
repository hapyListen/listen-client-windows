using System;

namespace ListenTools.ViewModels;

public sealed class ViewModelLocator
{
    private IServiceProvider _serviceProvider;

    public void SetServiceProvider(IServiceProvider provider)
    {
        _serviceProvider = provider;
    } 

}