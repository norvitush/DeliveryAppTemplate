using System;
using UnityEngine;

public class GameObjectOfTypeAttribute : PropertyAttribute
{
    public Type Type { get; }
    public bool AllowSceneObjects { get; }

    public GameObjectOfTypeAttribute(Type type, bool allowSceneObjects = true)
    {
        Type = type;
        AllowSceneObjects = allowSceneObjects;
    }

}
