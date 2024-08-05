using System;

public interface IDataKeeper: IModel
{
    object this[Type type] { get; }
}
