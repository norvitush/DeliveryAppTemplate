using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Helpers
{
    public enum FitType { InnerFit, OuterFit, ByHeight, ByWidth }

    private readonly static System.Random _rdm = new System.Random();

    private const string patternEmail = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    private const string COLORED_MESSAGE = "<color=#{1}>{0}</color>";

    //hashed camera
    private static Camera _camera;

    public static Camera Camera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }

    public static bool IsDestroyedOrNull(UnityEngine.Object reference)
    {
        try
        {
            var chekvar = reference.name;
        }
        catch (MissingReferenceException) // General Object like GameObject/Sprite etc
        {
            return true;
        }
        catch (MissingComponentException) // Specific for objects of type Component
        {
            return true;
        }
        catch (UnassignedReferenceException) // Specific for unassigned fields
        {
            return true;
        }
        catch (NullReferenceException) // Any other null reference like for local variables
        {
            return true;
        }
        return false;
    }


    public static void Clear(this Transform transform, params Type[] excludeComponentsOfType)
    {
        int cnt = transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            var child = transform.GetChild(i);

            bool canDestroyed = true;

            foreach (var type in excludeComponentsOfType)
            {
                if (child.TryGetComponent(type, out Component excludeComponent))
                {
                    canDestroyed = false;
                    break;
                }
            }

            if (canDestroyed) GameObject.Destroy(child.gameObject);
        }
    }

    public static void ClearOnly(this Transform transform, Type includeComponentsOfType)
    {
        int cnt = transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            var child = transform.GetChild(i);

            bool canDestroyed = false;

            if (child.TryGetComponent(includeComponentsOfType, out Component includeComponent))
            {
                canDestroyed = true;
            }

            if (canDestroyed) GameObject.Destroy(child.gameObject);
        }
    }

    public static void FitToParent(this RectTransform rawRect, Texture texture, RectTransform parentRect, FitType fitType, bool useOffset = true)
    {
        rawRect.sizeDelta = GetFittedSize(new Vector2(texture.width, texture.height), parentRect, fitType, useOffset);
    }

    public static Vector2 GetFittedSize(Vector2 rectTexture, RectTransform parentRect, FitType fitType, bool useOffset = true)
    {
        Vector2 rectParent = new Vector2(parentRect.rect.width, parentRect.rect.height);

        float rate = 1;
        float offset = useOffset ? 5 : 0;

        if (fitType == FitType.InnerFit) rate = Mathf.Min(rectParent.x / rectTexture.x, rectParent.y / rectTexture.y);
        if (fitType == FitType.OuterFit) rate = Mathf.Max(rectParent.x / rectTexture.x, rectParent.y / rectTexture.y);
        if (fitType == FitType.ByHeight) rate = rectParent.y / rectTexture.y;
        if (fitType == FitType.ByWidth) rate = rectParent.x / rectTexture.x;

        return new Vector2(rectTexture.x * (rate) + offset, rectTexture.y * (rate) + offset);
    }

    public static void ColorDebugLog(this string message, Color32 color)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(string.Format(COLORED_MESSAGE, message, ColorUtility.ToHtmlStringRGBA(color)));
#endif        
    }
    public static void ColorDebugLog(this string message, string hexColor)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(string.Format(COLORED_MESSAGE, message, hexColor));
#endif        
    }

    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            //Convert the byte array to hexadecimal string prior to.NET 5
            StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public static int GenerateRandomNo() => _rdm.Next(1000, 9999);

    public static bool IsEmailString(this string target) => string.IsNullOrEmpty(target) == false && Regex.IsMatch(target, patternEmail);
    public static string OrDefault(this string target, string defaultVal) => string.IsNullOrEmpty(target) == false ? target : defaultVal;

    public static void SaveToFile(string text, string name = null)
    {
        var filePath = Application.persistentDataPath + "/" + "gs" + "/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        if (string.IsNullOrEmpty(name)) name = $"save_{text.GetHashCode()}";
        File.WriteAllText(filePath + name, text);
        //fileData = File.ReadAllText(filePath + "catalog");
    }

    public static Coroutine OnFirstUpdate(this MonoBehaviour waiter, Action action)
    {
        if(waiter.isActiveAndEnabled)
            return waiter.StartCoroutine(WaitingFirstUpdate(action));
        else
        {
            var synteticWaiter = new GameObject("Coroutine waiter", typeof(CoroutineContainer)).GetComponent<CoroutineContainer>();
            return synteticWaiter.StartCoroutine(WaitingFirstUpdate(action, synteticWaiter.gameObject));
        }
    }
    private static IEnumerator WaitingFirstUpdate(Action action, GameObject forRemove = null)
    {
        yield return null;
        action?.Invoke();

        if (forRemove != null) GameObject.Destroy(forRemove);
    }
}
