using UnityEngine;

[CreateAssetMenu(fileName = "New AppGeneralSettings", menuName = "App General Settings", order = 1)]
public class AppGeneralSettingsSO : ScriptableObject
{
    public Vector2 MapDefaultStartPoint;
    public WindowsAnimator WindowsAnimatorPrefab;
}
