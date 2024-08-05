using UnityEditor;
using UnityEngine;

[System.Serializable, ]
public struct WindowAssociation
{   
    public WindowType WindowType;
    [SerializeField, GameObjectOfType(typeof(IView))] 
    public GameObject WindowObject;
    public MenuButtonType ButtonType;
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(WindowAssociation))]
public class WindowAssociationDrawer : PropertyDrawer
{

    private const float space = 5;

    public override void OnGUI(Rect rect,
                               SerializedProperty property,
                               GUIContent label)
    {
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var firstLineRect = new Rect(
            x: rect.x,
            y: rect.y,
            width: rect.width,
            height: EditorGUIUtility.singleLineHeight
        );
        DrawMainProperties(firstLineRect, property);

        EditorGUI.indentLevel = indent;
    }

    private void DrawMainProperties(Rect rect,
                                    SerializedProperty human)
    {
        rect.width = (rect.width - 2 * space) / 3;
        DrawProperty(rect, human.FindPropertyRelative("WindowType"));
        rect.x += rect.width + space;
        DrawProperty(rect, human.FindPropertyRelative("WindowObject"));
        rect.x += rect.width + space;
        DrawProperty(rect, human.FindPropertyRelative("ButtonType"));
    }

    private void DrawProperty(Rect rect,
                              SerializedProperty property)
    {
        EditorGUI.PropertyField(rect, property, GUIContent.none);
    }
}
#endif



[System.Serializable]
public struct SlidersAssociation
{
    public SliderPanelType Type;
    
    [SerializeField, GameObjectOfType(typeof(IView))] 
    public GameObject gameObject;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SlidersAssociation))]
public class SliderAssociationDrawer : PropertyDrawer
{

    private const float space = 5;

    public override void OnGUI(Rect rect,
                               SerializedProperty property,
                               GUIContent label)
    {
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var firstLineRect = new Rect(
            x: rect.x,
            y: rect.y,
            width: rect.width,
            height: EditorGUIUtility.singleLineHeight
        );
        DrawMainProperties(firstLineRect, property);

        EditorGUI.indentLevel = indent;
    }

    private void DrawMainProperties(Rect rect,
                                    SerializedProperty human)
    {
        rect.width = (rect.width - 2 * space) / 2;
        DrawProperty(rect, human.FindPropertyRelative("Type"));
        rect.x += rect.width + space;
        DrawProperty(rect, human.FindPropertyRelative("gameObject"));
    }

    private void DrawProperty(Rect rect,
                              SerializedProperty property)
    {
        EditorGUI.PropertyField(rect, property, GUIContent.none);
    }
}
#endif
