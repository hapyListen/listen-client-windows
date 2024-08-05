using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ListenTools;

public class GlobalContext
{
    private static GlobalContext? _instance;
    private static readonly object LockObj = new object();

    public static GlobalContext Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (LockObj)
                {
                    if (_instance == null)
                        _instance = new GlobalContext();
                }
            }

            return _instance;
        }
    }

    private GlobalContext()
    {
    }


    #region DI获取数据

    private IServiceProvider _serviceProvider;

    public void SetServiceProvider(IServiceProvider provider)
        => _serviceProvider = provider;

    public IHttpClientFactory GetHttpFactory()
    {
        using var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
    }

    public T CreateViewModel<T>() where T : ViewModels.Base.ViewModelBase
    {
        using var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    #endregion
}