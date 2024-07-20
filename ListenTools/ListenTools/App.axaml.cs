using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ListenTools.ViewModels;
using ListenTools.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ListenTools;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<HttpClient>();
        serviceCollection.RegisterViewModel();
        var provider = serviceCollection.BuildServiceProvider();
        var vmLocator = this.Resources["VmLocator"] as ViewModels.ViewModelLocator;
        vmLocator?.SetServiceProvider(provider);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

static class RegisterProvider
{
    public static void RegisterViewModel(this IServiceCollection collection)
    {
        collection.AddSingleton<ViewModels.MainWindowViewModel>();
    }
}