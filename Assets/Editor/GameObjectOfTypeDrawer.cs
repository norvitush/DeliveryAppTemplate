using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameObjectOfTypeAttribute))]
public class GameObjectOfTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        bool isFieldGO = IsFieldGameObject();

        if (!isFieldGO)
        {
            DrawError(position);
            return;
        }

        var goOfTypeAttribute = attribute as GameObjectOfTypeAttribute;
        var requiredType = goOfTypeAttribute.Type;

        CheckDragAndDrops(position, requiredType);
        CheckValues(property, requiredType);
        DrawObjectfield(property, position, label, goOfTypeAttribute.AllowSceneObjects);
        EditorGUI.indentLevel = indent; ;
    }

    private void CheckValues(SerializedProperty property, Type requiredType)
    {
        if(property.objectReferenceValue != null)
        {
            if(!IsValidObject(property.objectReferenceValue, requiredType))
            {
                property.objectReferenceValue = null;
            }
        }
    }

    private void DrawObjectfield(SerializedProperty property, Rect position, GUIContent lable, bool allowSceneObjects)
    {

        EditorGUI.PropertyField(position, property, lable);

    }

    private void DrawError(Rect position)
    {
        EditorGUI.HelpBox(position,
                          message: $"GameObjectOfTypeAttribute works only with GameObjects",
                          MessageType.Error);
    }

    private bool IsFieldGameObject()
    {
        return fieldInfo.FieldType == typeof(GameObject)
            || typeof(IEnumerable<GameObject>).IsAssignableFrom(fieldInfo.FieldType);
    }

    private void CheckDragAndDrops(Rect position, Type requiredType)
    {
        if(position.Contains(Event.current.mousePosition))
        {
           
            var draggedObjCount = DragAndDrop.objectReferences.Length;
            for (int i = 0; i < draggedObjCount; i++)
            {
              
                if (!IsValidObject(DragAndDrop.objectReferences[i], requiredType))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    break;
                }
            }
        }

    }

    private bool IsValidObject(UnityEngine.Object o, Type requiredType)
    {
        bool result = false;

        var go = o as GameObject;

        if(go != null)
        {
            result = go.GetComponent(requiredType) != null;
        }

        return result;
    }
}