using System;
using System.Collections.Generic;

public class ServiceLocator : IServiceProvider
{
    public readonly static ServiceLocator Instance = new ServiceLocator();

    private readonly static Dictionary<Type, Func<object>>
        _services = new Dictionary<Type, Func<object>>();

    private ServiceLocator() { }

    public void Register<T>(Func<T> resolver)
    {
        _services[typeof(T)] = () => resolver();
    }

    public T Resolve<T>()
    {
        return _services.TryGetValue(typeof(T), out var result)? (T)result() : default(T);
    }

    public void Reset()
    {
        _services.Clear();
    }

}
