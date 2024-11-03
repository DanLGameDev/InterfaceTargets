using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace DGP.InterfaceTargets.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceTargets<>))]
    public class InterfaceTargetsDrawer : PropertyDrawer
    {
        private const float ErrorBoxHeight = 40f;
        private const float Padding = 2f;
        private const float ButtonWidth = 20f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetsProp = property.FindPropertyRelative("targets");
            var isRequired = Attribute.IsDefined(fieldInfo, typeof(RequireTargetAttribute));
            
            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                Rect listRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, 0);

                for (int i = 0; i < targetsProp.arraySize; i++)
                {
                    Rect elementRect = new Rect(listRect.x, listRect.y + i * (EditorGUIUtility.singleLineHeight + Padding), 
                        listRect.width - ButtonWidth, EditorGUIUtility.singleLineHeight);
                    Rect removeButtonRect = new Rect(elementRect.xMax, elementRect.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

                    var elementProp = targetsProp.GetArrayElementAtIndex(i);
                    var newValue = EditorGUI.ObjectField(elementRect, elementProp.objectReferenceValue, typeof(Object), true);
                    
                    if (newValue != elementProp.objectReferenceValue)
                    {
                        if (IsValidTarget(property, newValue, out var validObject) || (!isRequired && newValue == null))
                        {
                            elementProp.objectReferenceValue = validObject;
                        }
                    }

                    if (GUI.Button(removeButtonRect, "-"))
                    {
                        targetsProp.DeleteArrayElementAtIndex(i);
                        i--;
                    }
                }

                Rect addButtonRect = new Rect(listRect.x, listRect.y + targetsProp.arraySize * (EditorGUIUtility.singleLineHeight + Padding), 
                    listRect.width, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(addButtonRect, "Add Element"))
                {
                    targetsProp.InsertArrayElementAtIndex(targetsProp.arraySize);
                    targetsProp.GetArrayElementAtIndex(targetsProp.arraySize - 1).objectReferenceValue = null;
                }

                EditorGUI.indentLevel--;
            }

            Rect errorBoxRect = new Rect(position.x, position.yMax - ErrorBoxHeight, position.width, ErrorBoxHeight);
            if (HasInvalidTargets(targetsProp, isRequired))
            {
                EditorGUI.HelpBox(errorBoxRect, 
                    isRequired
                        ? $"All targets must implement {fieldInfo.FieldType.GetGenericArguments()[0].Name} and cannot be empty"
                        : $"All non-empty targets must implement {fieldInfo.FieldType.GetGenericArguments()[0].Name}",
                    MessageType.Error);
            }

            EditorGUI.EndProperty();
        }

        private bool IsValidTarget(SerializedProperty property, Object targetValue, out Object validObject)
        {
            validObject = null;
            if (targetValue == null)
            {
                return true; // Null is considered valid unless RequireTarget is present
            }

            var interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];
            
            if (interfaceType.IsInstanceOfType(targetValue))
            {
                validObject = targetValue;
                return true;
            }

            if (targetValue is Component component)
            {
                validObject = component.GetComponent(interfaceType);
                return validObject != null;
            }

            if (targetValue is GameObject gameObject)
            {
                validObject = gameObject.GetComponent(interfaceType);
                return validObject != null;
            }

            return false;
        }

        private bool HasInvalidTargets(SerializedProperty targetsProp, bool isRequired)
        {
            for (int i = 0; i < targetsProp.arraySize; i++)
            {
                var elementProp = targetsProp.GetArrayElementAtIndex(i);
                if (elementProp.objectReferenceValue == null)
                {
                    if (isRequired) return true;
                }
                else if (!IsValidTarget(targetsProp, elementProp.objectReferenceValue, out _))
                {
                    return true;
                }
            }
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var targetsProp = property.FindPropertyRelative("targets");
            float height = EditorGUIUtility.singleLineHeight; // Foldout height

            if (property.isExpanded)
            {
                height += (targetsProp.arraySize + 1) * (EditorGUIUtility.singleLineHeight + Padding); // List items + Add button
                
                if (HasInvalidTargets(targetsProp, Attribute.IsDefined(fieldInfo, typeof(RequireTargetAttribute))))
                {
                    height += ErrorBoxHeight + Padding;
                }
            }

            return height;
        }
    }
}