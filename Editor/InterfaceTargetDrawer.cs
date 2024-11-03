using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace DGP.InterfaceTargets.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceTarget<>))]
    public class InterfaceTargetDrawer : PropertyDrawer
    {
        private const float ErrorBoxHeight = 40f;
        private const float Padding = 2f;

        private Rect objectFieldRect;
        private Rect errorBoxRect;
        
        static InterfaceTargetDrawer()
        {
            EditorApplication.update += () => { };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DropAreaGUI(position, property);
            EditorGUI.BeginProperty(position, label, property);

            var targetProp = property.FindPropertyRelative("target");
            var isRequired = Attribute.IsDefined(fieldInfo, typeof(RequireTargetAttribute));
            
            if (targetProp.objectReferenceValue is GameObject gameObject)
            {
                if (IsValidTarget(property, gameObject, out var validObject))
                {
                    targetProp.objectReferenceValue = validObject;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            
            // Calculate rects
            objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            errorBoxRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Padding, position.width, ErrorBoxHeight);
            
            EditorGUI.PropertyField(objectFieldRect, targetProp, label);
            
            if (targetProp.objectReferenceValue == null)
            {
                if (isRequired)
                {
                    EditorGUI.HelpBox(
                        errorBoxRect,
                        "This field is required",
                        MessageType.Error
                    );
                }
            }
            else if (!IsValidTarget(property, targetProp.objectReferenceValue, out _))
            {
                EditorGUI.HelpBox(
                    errorBoxRect,
                    $"Target must implement {fieldInfo.FieldType.GetGenericArguments()[0].Name}",
                    MessageType.Error
                );
            }

            EditorGUI.EndProperty();
        }

        private void DropAreaGUI(Rect dropArea, SerializedProperty property)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition) || DragAndDrop.objectReferences.Length > 1)
                        return;

                    Object draggedObject = DragAndDrop.objectReferences[0];

                    var isValidTarget = IsValidTarget(property, draggedObject, out var validObject);
                    
                    if (!isValidTarget) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    } else {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        
                        if (evt.type == EventType.DragPerform) {
                            var targetProp = property.FindPropertyRelative("target");
                            targetProp.objectReferenceValue = validObject;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    
                    break;
            }
        }
        
        private bool IsValidTarget(SerializedProperty property, Object targetValue, out Object validObject)
        {
            var interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];
            
            if (interfaceType.IsInstanceOfType(targetValue)) {
                validObject = targetValue;
                return true;
            }

            if (targetValue is Component component) {
                validObject = component.GetComponent(interfaceType);
                return validObject != null;
            }

            if (targetValue is GameObject gameObject) {
                validObject = gameObject.GetComponent(interfaceType);
                return validObject != null;
            }

            validObject = null;
            return false;
        }

        private bool HasError(SerializedProperty property)
        {
            var isRequired = Attribute.IsDefined(fieldInfo, typeof(RequireTargetAttribute));
            var targetProp = property.FindPropertyRelative("target");
            
            if (targetProp.objectReferenceValue == null)
                return isRequired;
            
            return !IsValidTarget(property, targetProp.objectReferenceValue, out _);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return HasError(property) 
                ? EditorGUIUtility.singleLineHeight + ErrorBoxHeight + Padding * 2
                : EditorGUIUtility.singleLineHeight;
        }
    }
}