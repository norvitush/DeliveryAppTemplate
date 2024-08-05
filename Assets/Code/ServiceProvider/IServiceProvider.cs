using System;

public interface IServiceProvider: IReadonlyServiceProvider
{
    void Register<T>(Func<T> resolver);
    void Reset();
}
