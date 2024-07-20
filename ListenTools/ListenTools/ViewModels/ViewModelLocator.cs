using System;
using Microsoft.Extensions.DependencyInjection;

namespace ListenTools.ViewModels;

public sealed class ViewModelLocator
{
    private IServiceProvider _serviceProvider;

    public void SetServiceProvider(IServiceProvider provider)
    {
        _serviceProvider = provider;
    }

    public MainWindowViewModel MainVm
    {
        get
        {
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        }
    }
}